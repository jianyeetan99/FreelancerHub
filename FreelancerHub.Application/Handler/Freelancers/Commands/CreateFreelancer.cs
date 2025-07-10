using FluentResults;
using FluentValidation;
using FreelancerHub.Application.DTOs;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;
using MediatR;

namespace FreelancerHub.Application.Handler.Freelancer.Commands;
public record CreateFreelancerCommand(CreateFreelancerDto Dto, Guid UserId) : IRequest<Result<Guid>>;

public class CreateFreelancerValidator : AbstractValidator<CreateFreelancerDto>
{
    public CreateFreelancerValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleForEach(x => x.Skills).NotEmpty();
        RuleForEach(x => x.Hobbies).NotEmpty();
    }
}

public class CreateFreelancerHandler(IFreelancerRepository repo)
    : IRequestHandler<CreateFreelancerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFreelancerCommand request, CancellationToken ct)
    {
        var freelancer = new Domain.Entities.Freelancer
        {
            Id = Guid.NewGuid(),
            Username = request.Dto.Username,
            Email = request.Dto.Email,
            PhoneNumber = request.Dto.PhoneNumber,
            Skills = request.Dto.Skills.Select(s => new Skill { SkillId = Guid.NewGuid(), SkillName = s }).ToList(),
            Hobbies = request.Dto.Hobbies.Select(h => new Hobby { HobbyId = Guid.NewGuid(), HobbyName = h }).ToList()
        };

        var id = await repo.CreateAsync(freelancer);
        return Result.Ok(id);
    }
}