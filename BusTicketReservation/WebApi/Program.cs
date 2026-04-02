using Application.Services;
using Application.Services.Admin;
using Application.Services.Auth;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────────────────────────

builder.Services.AddControllers();

// ── Database ──────────────────────────────────────────────────────────────────

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── JWT Authentication ────────────────────────────────────────────────────────

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured in appsettings");

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var authService = context.HttpContext.RequestServices
                    .GetRequiredService<IAuthService>();

                var userIdClaim =
                    context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    context.Fail("User ID claim is missing or invalid");
                    return;
                }

                var isValid = await authService.ValidateUserAsync(userId);
                if (!isValid)
                    context.Fail("User not found or inactive");
            }
        };
    });

// ── Authorization ─────────────────────────────────────────────────────────────

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ── Dependency Injection ──────────────────────────────────────────────────────

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ISeatAvailabilityService, SeatAvailabilityService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISeatLockService, SeatLockService>();
builder.Services.AddScoped<IUserBookingService, UserBookingService>();

builder.Services.AddScoped<IAdminBusService, AdminBusService>();
builder.Services.AddScoped<IAdminRouteService, AdminRouteService>();
builder.Services.AddScoped<IAdminScheduleService, AdminScheduleService>();
builder.Services.AddScoped<IAdminBookingService, AdminBookingService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();

// ── Background Services ───────────────────────────────────────────────────────

builder.Services.AddHostedService<SeatLockCleanupService>();

// ── CORS ──────────────────────────────────────────────────────────────────────

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ── OpenAPI  (.NET 10 native document generation + Swashbuckle UI) ────────────
//
//  Stack:
//    Microsoft.AspNetCore.OpenApi  10.x  -> AddOpenApi() / MapOpenApi()
//                                           generates /openapi/v1.json
//    Swashbuckle.AspNetCore.SwaggerUI    -> UseSwaggerUI() renders interactive UI
//
//  csproj packages required (nothing else):
//    Microsoft.AspNetCore.OpenApi                   10.x
//    Microsoft.AspNetCore.Authentication.JwtBearer  10.x
//    Swashbuckle.AspNetCore.SwaggerUI               10.x
//    Npgsql.EntityFrameworkCore.PostgreSQL          10.x
//    BCrypt.Net-Next                                4.x
//
//  Do NOT add a standalone Microsoft.OpenApi package reference.
//  Swashbuckle 10 pulls OpenAPI.NET 2.x transitively; mixing versions
//  causes "Models namespace does not exist" compile errors.
//
//  BREAKING CHANGE vs .NET 8/9:
//    OpenApiReference and the Reference property on OpenApiSecurityScheme were
//    removed in OpenAPI.NET 2.x.
//    Use:  new OpenApiSecuritySchemeReference("Bearer", document)

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        // Ensure Components exists
        document.Components ??= new OpenApiComponents();

        // FIXED TYPE HERE
        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        // Register Bearer scheme
        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter JWT token"
            };

        // Apply Security Globally
        if (document.Paths != null)
        {
            foreach (var path in document.Paths.Values)
            {
                if (path?.Operations == null)
                    continue;

                foreach (var operation in path.Operations.Values)
                {
                    operation.Security ??=
                        new List<OpenApiSecurityRequirement>();

                    operation.Security.Add(
                        new OpenApiSecurityRequirement
                        {
                            [
                                new OpenApiSecuritySchemeReference(
                                    "Bearer",
                                    document
                                )
                            ] = new List<string>()
                        });
                }
            }
        }

        return Task.CompletedTask;
    });
});

// ── Build ─────────────────────────────────────────────────────────────────────

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────────

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();          // /openapi/v1.json

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Bus Ticket API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Bus Ticket API";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Database Migration + Admin Seed ───────────────────────────────────────────

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var adminExists = await db.Users.AnyAsync(u => u.Role == UserRole.Admin && !u.IsDeleted);
    if (!adminExists)
    {
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@busticket.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
            FullName = "System Administrator",
            MobileNumber = "0000000000",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        Console.WriteLine("Default admin seeded — username: admin  password: Admin@1234");
    }
}

app.Run();