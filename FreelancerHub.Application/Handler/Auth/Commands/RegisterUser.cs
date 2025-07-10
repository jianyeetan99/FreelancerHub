using FluentResults;
using FluentValidation;
using FreelancerHub.Application.DTOs.Auth;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;
using MediatR;

namespace FreelancerHub.Application.Handler.Auth.Commands;

public record RegisterUserCommand(RegisterUserDto Dto) : IRequest<Result<Guid>>;


public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}


public class RegisterUserHandler(IUserRepository repo) : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var existing = await repo.GetByEmailAsync(request.Dto.Email);
        if (existing != null)
            return Result.Fail("Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Dto.Username,
            Email = request.Dto.Email,
            Role = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password)
        };

        var id = await repo.CreateAsync(user);
        return Result.Ok(id);
    }
}