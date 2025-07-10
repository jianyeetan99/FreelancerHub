using FreelancerHub.Application.DTOs.Auth;

namespace FreelancerHub.Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}