using FluentResults;
using FluentValidation;
using FreelancerHub.Application.DTOs.Auth;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;
using MediatR;

namespace FreelancerHub.Application.Handler.Auth.Commands;

public record LoginUserCommand(LoginUserDto Dto) : IRequest<Result<AuthResultDto>>;

public class LoginUserValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}


public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<AuthResultDto>>
{
    private readonly IUserRepository _repo;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public LoginUserHandler(
        IUserRepository repo,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepo)
    {
        _repo = repo;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<Result<AuthResultDto>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByEmailAsync(request.Dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Dto.Password, user.PasswordHash))
            return Result.Fail("Invalid email or password.");

        var accessToken = _tokenService.GenerateToken(user);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id.ToString(),
            Token = Guid.NewGuid().ToString(),
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepo.SaveAsync(refreshToken);

        return Result.Ok(new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        });
    }
}
