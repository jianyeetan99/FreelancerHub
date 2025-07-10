using FluentResults;
using FluentValidation;
using FreelancerHub.Application.DTOs;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;
using MediatR;

namespace FreelancerHub.Application.Handler.Freelancer.Commands;

public record UpdateFreelancerCommand(Guid Id, UpdateFreelancerDto Dto, Guid UserId) : IRequest<Result>;

public class UpdateFreelancerValidator : AbstractValidator<UpdateFreelancerDto>
{
    public UpdateFreelancerValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleForEach(x => x.Skills).NotEmpty();
        RuleForEach(x => x.Hobbies).NotEmpty();
    }
}

public class UpdateFreelancerHandler(IFreelancerRepository repo) : IRequestHandler<UpdateFreelancerCommand, Result>
{
    public async Task<Result> Handle(UpdateFreelancerCommand request, CancellationToken ct)
    {
        var freelancer = await repo.GetByIdAsync(request.Id);
        if (freelancer == null)
            return Result.Fail("Freelancer not found.");

        freelancer.Username = request.Dto.Username;
        freelancer.Email = request.Dto.Email;
        freelancer.PhoneNumber = request.Dto.PhoneNumber;

        freelancer.Skills = request.Dto.Skills.Select(s => new Skill
        {
            SkillId = Guid.NewGuid(),
            SkillName = s
        }).ToList();

        freelancer.Hobbies = request.Dto.Hobbies.Select(h => new Hobby
        {
            HobbyId = Guid.NewGuid(),
            HobbyName = h
        }).ToList();

        var updated = await repo.UpdateAsync(freelancer);
        return updated ? Result.Ok() : Result.Fail("Update failed.");
    }
}