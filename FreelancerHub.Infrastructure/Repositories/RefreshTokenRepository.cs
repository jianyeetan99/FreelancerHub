using System.Data;
using Dapper;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;
using FreelancerHub.Infrastructure.Common;

namespace FreelancerHub.Infrastructure.Repositories;

public class RefreshTokenRepository(DapperContext context) : IRefreshTokenRepository
{
    public async Task SaveAsync(RefreshToken token)
    {
        var sql = "INSERT INTO RefreshTokens (UserId, Token, ExpiryDate) VALUES (@UserId, @Token, @ExpiryDate)";
        using IDbConnection conn = context.Freelancer();

        await conn.ExecuteAsync(sql, token);
    }

    // public Task<RefreshToken?> GetByTokenAsync(string token)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public Task InvalidateAsync(string token)
    // {
    //     throw new NotImplementedException();
    // }
}