using BusReservation.Application.Interfaces;
using BusReservation.Application.Services;
using BusReservation.Infrastructure.Persistence;
using BusReservation.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // your Angular app URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory", true);
if (useInMemory)
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("BusReservationDb"));
else
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));
}

// Repositories
builder.Services.AddScoped<IBusScheduleRepository, BusScheduleRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();



// Services
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

app.UseCors("AllowAngularApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SampleDataSeeder.SeedAsync(db);
}

app.Run();