using FreelancerHub.Domain.Entities;

namespace FreelancerHub.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task SaveAsync(RefreshToken token);
    // Task<RefreshToken?> GetByTokenAsync(string token);
    // Task InvalidateAsync(string token);
}