using System.Data;
using Dapper;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;
using FreelancerHub.Infrastructure.Common;

namespace FreelancerHub.Infrastructure.Repositories;

public class FreelancerRepository(DapperContext context) : IFreelancerRepository
{
    public async Task<List<Freelancer>> GetAllAsync()
    {
        var sql = @"
        SELECT f.Id, f.Username, f.Email, f.PhoneNumber, f.IsArchived,
               s.Id AS SkillId, s.Name AS SkillName, s.FreelancerId AS SkillFreelancerId,
                h.Id AS HobbyId, h.Name AS HobbyName, h.FreelancerId AS HobbyFreelancerId
        FROM Freelancers f
        LEFT JOIN Skills s ON s.FreelancerId = f.Id
        LEFT JOIN Hobbies h ON h.FreelancerId = f.Id
        WHERE f.IsArchived = 0";

        var freelancerDict = new Dictionary<Guid, Freelancer>();
        using IDbConnection conn = context.Freelancer();

        var result = await conn.QueryAsync<Freelancer, Skill, Hobby, Freelancer>(
            sql,
            (freelancer, skill, hobby) =>
            {
                if (!freelancerDict.TryGetValue(freelancer.Id, out var existing))
                {
                    existing = freelancer;
                    existing.Skills = new();
                    existing.Hobbies = new();
                    freelancerDict.Add(existing.Id, existing);
                }

                if (skill != null && skill.SkillId != Guid.Empty &&
                    !existing.Skills.Any(s => s.SkillId == skill.SkillId))
                    existing.Skills.Add(skill);

                if (hobby != null && hobby.HobbyId != Guid.Empty &&
                    !existing.Hobbies.Any(h => h.HobbyId == hobby.HobbyId))
                    existing.Hobbies.Add(hobby);

                return existing;
            },
            splitOn: "SkillId,HobbyId"
        );

        return freelancerDict.Values.ToList();
    }

    public async Task<Freelancer?> GetByIdAsync(Guid id)
    {
        const string sql = @"
        SELECT 
            f.Id, f.Username, f.Email, f.PhoneNumber, f.IsArchived,

            s.Id AS SkillId, s.Name AS SkillName, s.FreelancerId AS SkillFreelancerId,
            h.Id AS HobbyId, h.Name AS HobbyName, h.FreelancerId AS HobbyFreelancerId

        FROM Freelancers f
        LEFT JOIN Skills s ON f.Id = s.FreelancerId
        LEFT JOIN Hobbies h ON f.Id = h.FreelancerId
        WHERE f.Id = @Id;";

        var freelancerDict = new Dictionary<Guid, Freelancer>();
        using IDbConnection conn = context.Freelancer();

        var result = await conn.QueryAsync<Freelancer, Skill, Hobby, Freelancer>(
            sql,
            (freelancer, skill, hobby) =>
            {
                if (!freelancerDict.TryGetValue(freelancer.Id, out var existing))
                {
                    existing = freelancer;
                    existing.Skills = new List<Skill>();
                    existing.Hobbies = new List<Hobby>();
                    freelancerDict[freelancer.Id] = existing;
                }

                if (skill != null && skill.SkillId != Guid.Empty &&
                    !existing.Skills.Any(s => s.SkillId == skill.SkillId))
                    existing.Skills.Add(skill);

                if (hobby != null && hobby.HobbyId != Guid.Empty &&
                    !existing.Hobbies.Any(h => h.HobbyId == hobby.HobbyId))
                    existing.Hobbies.Add(hobby);

                return existing;
            },
            new { Id = id },
            splitOn: "SkillId,HobbyId"
        );

        return result.FirstOrDefault();
    }

    public async Task<List<Freelancer>> SearchAsync(string keyword)
    {
        using var conn = context.Freelancer();

        var freelancers = (await conn.QueryAsync<Freelancer>(
            @"SELECT * FROM Freelancers 
          WHERE Username LIKE @K OR Email LIKE @K",
            new { K = $"%{keyword}%" })).ToList();

        if (!freelancers.Any())
            return freelancers;

        var ids = freelancers.Select(f => f.Id).ToArray();

        var skills = (await conn.QueryAsync<Skill>(
            @"SELECT Id AS SkillId, Name AS SkillName, FreelancerId AS SkillFreelancerId FROM Skills 
          WHERE FreelancerId IN @Ids", new { Ids = ids })).ToList();

        var hobbies = (await conn.QueryAsync<Hobby>(
            @"SELECT Id AS HobbyId, Name AS HobbyName, FreelancerId AS HobbyFreelancerId FROM Hobbies 
          WHERE FreelancerId IN @Ids", new { Ids = ids })).ToList();

        foreach (var f in freelancers)
        {
            f.Skills = skills.Where(s => s.SkillFreelancerId == f.Id).ToList();
            f.Hobbies = hobbies.Where(h => h.HobbyFreelancerId == f.Id).ToList();
        }
        return freelancers;
    }

    public async Task<Guid> CreateAsync(Freelancer freelancer)
    {
        freelancer.Id = Guid.NewGuid();

        using IDbConnection conn = context.Freelancer();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            var freelancerSql = @"INSERT INTO Freelancers (Id, Username, Email, PhoneNumber, IsArchived)
                              VALUES (@Id, @Username, @Email, @PhoneNumber, 0)";
            await conn.ExecuteAsync(freelancerSql, freelancer, transaction);

            var skillSql = "INSERT INTO Skills (Id, Name, FreelancerId) VALUES (@Id, @Name, @FreelancerId)";
            var skillParams = freelancer.Skills.Select(s => new
            {
                Id = Guid.NewGuid(),
                Name = s.SkillName,
                FreelancerId = freelancer.Id
            });
            await conn.ExecuteAsync(skillSql, skillParams, transaction);

            var hobbySql = "INSERT INTO Hobbies (Id, Name, FreelancerId) VALUES (@Id, @Name, @FreelancerId)";
            var hobbyParams = freelancer.Hobbies.Select(h => new
            {
                Id = Guid.NewGuid(),
                Name = h.HobbyName,
                FreelancerId = freelancer.Id
            });
            await conn.ExecuteAsync(hobbySql, hobbyParams, transaction);

            transaction.Commit();
            return freelancer.Id;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Freelancer freelancer)
    {
        var updateFreelancerSql = @"
            UPDATE Freelancers 
            SET Username = @Username, Email = @Email, PhoneNumber = @PhoneNumber
            WHERE Id = @Id;";

        var deleteSkillsSql = @"
            DELETE FROM Skills 
            WHERE FreelancerId = @FreelancerId;";

        var insertSkillsSql = @"
            INSERT INTO Skills (Id, Name, FreelancerId)
            VALUES (@Id, @Name, @FreelancerId);";

        var deleteHobbiesSql = @"
            DELETE FROM Hobbies 
            WHERE FreelancerId = @FreelancerId;";

        var insertHobbiesSql = @"
            INSERT INTO Hobbies (Id, Name, FreelancerId)
            VALUES (@Id, @Name, @FreelancerId);";

        using IDbConnection conn = context.Freelancer();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            await conn.ExecuteAsync(updateFreelancerSql, freelancer, transaction);

            await conn.ExecuteAsync(deleteSkillsSql, new { FreelancerId = freelancer.Id }, transaction);

            var skills = freelancer.Skills.Select(s => new
            {
                Id = s.SkillId == Guid.Empty ? Guid.NewGuid() : s.SkillId,
                Name = s.SkillName,
                FreelancerId = freelancer.Id
            });

            await conn.ExecuteAsync(insertSkillsSql, skills, transaction);

            await conn.ExecuteAsync(deleteHobbiesSql, new { FreelancerId = freelancer.Id }, transaction);

            var hobbies = freelancer.Hobbies.Select(h => new
            {
                Id = h.HobbyId == Guid.Empty ? Guid.NewGuid() : h.HobbyId,
                Name = h.HobbyName,
                FreelancerId = freelancer.Id
            });

            await conn.ExecuteAsync(insertHobbiesSql, hobbies, transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sql = @"
            DELETE FROM Hobbies WHERE FreelancerId = @Id;
            DELETE FROM Skills WHERE FreelancerId = @Id;
            DELETE FROM Freelancers WHERE Id = @Id";
        using IDbConnection conn = context.Freelancer();
        var affected = await conn.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var sql = "UPDATE Freelancers SET IsArchived = 1 WHERE Id = @Id";
        using IDbConnection conn = context.Freelancer();
        var affected = await conn.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }

    public async Task<bool> UnarchiveAsync(Guid id)
    {
        var sql = "UPDATE Freelancers SET IsArchived = 0 WHERE Id = @Id";
        using IDbConnection conn = context.Freelancer();
        var affected = await conn.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}