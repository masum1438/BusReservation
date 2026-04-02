using Application.DTOs.Auth;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync();
    Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<UserDto> GetCurrentUserAsync();
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ValidateUserAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    // ── Login ──────────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.FindAsync(u =>
            u.Email == loginDto.Email && !u.IsDeleted && u.IsActive);
        var user = users.FirstOrDefault();

        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            return Fail("Invalid username or password");

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await userRepo.UpdateAsync(user);

        var (token, refreshToken) = await IssueTokensAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Success("Login successful", token, refreshToken, user);
    }

    // ── Register ───────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var userRepo = _unitOfWork.Repository<User>();

        var existing = await userRepo.FindAsync(u =>
            (u.Username == registerDto.Username || u.Email == registerDto.Email) && !u.IsDeleted);

        if (existing.Any())
            return Fail("Username or email already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password),
            FullName = registerDto.FullName,
            MobileNumber = registerDto.MobileNumber,
            // FIX: Set defaults that were missing before
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await userRepo.AddAsync(user);

        var (token, refreshToken) = await IssueTokensAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Success("Registration successful", token, refreshToken, user);
    }

    // ── Refresh Token ──────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var repo = _unitOfWork.Repository<RefreshToken>();

        // FIX: Also check RevokedAt == null — previously revoked tokens were still accepted
        var tokens = await repo.FindAsync(rt =>
            rt.Token == refreshToken &&
            rt.RevokedAt == null &&
            rt.ExpiresAt > DateTime.UtcNow &&
            !rt.IsDeleted);

        var token = tokens.FirstOrDefault();
        if (token == null)
            return Fail("Invalid or expired refresh token");

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(token.UserId);
        if (user == null || !user.IsActive)
            return Fail("User not found or inactive");

        // Revoke old token (soft revoke, keep record)
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = GetClientIp();
        await repo.UpdateAsync(token);

        var (newJwt, newRefresh) = await IssueTokensAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Success("Token refreshed successfully", newJwt, newRefresh, user);
    }

    // ── Logout ─────────────────────────────────────────────────────────────────

    // FIX: Actually revoke all active refresh tokens for the current user
    public async Task<bool> LogoutAsync()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return false;

        var repo = _unitOfWork.Repository<RefreshToken>();
        var activeTokens = await repo.FindAsync(rt =>
            rt.UserId == currentUser.Id &&
            rt.RevokedAt == null &&
            rt.ExpiresAt > DateTime.UtcNow &&
            !rt.IsDeleted);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = GetClientIp();
            await repo.UpdateAsync(token);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // ── Change Password ────────────────────────────────────────────────────────

    public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return false;

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(currentUser.Id);
        if (user == null) return false;

        if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await userRepo.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // ── Get Current User ───────────────────────────────────────────────────────

    public async Task<UserDto> GetCurrentUserAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            return null!;

        return (await GetUserByIdAsync(id))!;
    }

    // ── Get User By Id ─────────────────────────────────────────────────────────

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId);
        return user != null && !user.IsDeleted ? MapToUserDto(user) : null;
    }

    // ── Revoke Token ───────────────────────────────────────────────────────────

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var repo = _unitOfWork.Repository<RefreshToken>();
        var tokens = await repo.FindAsync(rt =>
            rt.Token == refreshToken && rt.RevokedAt == null);
        var token = tokens.FirstOrDefault();

        if (token == null) return false;

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = GetClientIp();
        await repo.UpdateAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // FIX: ValidateUserAsync was NotImplementedException — now properly implemented
    public async Task<bool> ValidateUserAsync(Guid userId)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId);
        return user != null && user.IsActive && !user.IsDeleted;
    }

    // ── Private Helpers ────────────────────────────────────────────────────────

    private async Task<(string jwt, string refresh)> IssueTokensAsync(User user)
    {
        var jwt = GenerateJwtToken(user);
        var refresh = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            // FIX: Id was never set before, causing empty GUIDs in DB
            Id = Guid.NewGuid(),
            Token = refresh,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7")),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetClientIp()
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenEntity);
        return (jwt, refresh);
    }

    // FIX: Role claim now uses actual user role, not hardcoded "User"
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            // FIX: was hardcoded "User" — now uses actual role
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60")),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    //private static string GenerateRefreshToken()
    //    => Convert.ToBase64String(Guid.NewGuid().ToByteArray() + Guid.NewGuid().ToByteArray());

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    private static string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    private static bool VerifyPassword(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);

    private string? GetClientIp()
        => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    private static UserDto MapToUserDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName,
        MobileNumber = user.MobileNumber,
        Role = user.Role.ToString()
    };

    private static AuthResponseDto Fail(string message) => new() { IsSuccess = false, Message = message };

    private static AuthResponseDto Success(string message, string token, string refreshToken, User user) => new()
    {
        IsSuccess = true,
        Message = message,
        Token = token,
        RefreshToken = refreshToken,
        User = MapToUserDto(user)
    };
}