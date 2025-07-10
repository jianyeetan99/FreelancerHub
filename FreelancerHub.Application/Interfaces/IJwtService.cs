namespace FreelancerHub.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string role);
}
