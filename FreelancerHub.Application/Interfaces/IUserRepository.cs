using FreelancerHub.Domain.Entities;

namespace FreelancerHub.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<Guid> CreateAsync(User user);
}