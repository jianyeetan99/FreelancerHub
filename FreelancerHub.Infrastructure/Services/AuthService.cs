using System.Data;
using Dapper;
using FreelancerHub.Application.DTOs.Auth;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;
using FreelancerHub.Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FreelancerHub.Infrastructure.Services;

public class AuthService(DapperContext context, IConfiguration config) : IAuthService
{
    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var hashed = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashed
        };

        using IDbConnection conn = context.Freelancer();
        var sql = @"INSERT INTO Users (Id, Username, Email, PasswordHash)
                    VALUES (@Id, @Username, @Email, @PasswordHash)";
        var affected = await conn.ExecuteAsync(sql, user);
        return affected > 0;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        using IDbConnection conn = context.Freelancer();
        var user = await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email", new { request.Email });

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthResponse
        {
            AccessToken = tokenHandler.WriteToken(token),
            RefreshToken = GenerateRefreshToken() // can skip if not persisting
        };
    }

    private string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
