using System.Data;
using Dapper;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;
using FreelancerHub.Infrastructure.Common;

namespace FreelancerHub.Infrastructure.Repositories;

public class UserRepository(DapperContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        using IDbConnection conn = context.Freelancer();
        var sql = "SELECT * FROM Users WHERE Email = @Email";
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<Guid> CreateAsync(User user)
    {
        using IDbConnection conn = context.Freelancer();
        var sql = @"INSERT INTO Users (Id, Username, Email, PasswordHash, Role)
                    VALUES (@Id, @Username, @Email, @PasswordHash, @Role)";
        await conn.ExecuteAsync(sql, user);
        return user.Id;
    }
}