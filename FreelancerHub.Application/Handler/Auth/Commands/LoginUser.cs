using FluentResults;
using FluentValidation;
using FreelancerHub.Application.DTOs.Auth;
using FreelancerHub.Application.Interfaces;
using MediatR;

namespace FreelancerHub.Application.Handler.Auth.Commands;

public record LoginUserCommand(LoginUserDto Dto) : IRequest<Result<string>>;

public class LoginUserValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}


public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<string>>
{
    private readonly IUserRepository _repo;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(IUserRepository repo, ITokenService tokenService)
    {
        _repo = repo;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByEmailAsync(request.Dto.Email);
        if (user == null)
            return Result.Fail("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Dto.Password, user.PasswordHash))
            return Result.Fail("Invalid credentials.");

        var token = _tokenService.GenerateToken(user);
        return Result.Ok(token);
    }
}