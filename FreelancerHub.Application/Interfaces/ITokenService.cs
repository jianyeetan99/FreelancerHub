using FreelancerHub.Domain.Entities;

namespace FreelancerHub.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}