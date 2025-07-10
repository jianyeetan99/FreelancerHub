using FluentResults;
using FreelancerHub.Application.DTOs;
using FreelancerHub.Application.Interfaces;
using MediatR;

namespace FreelancerHub.Application.Handler.Freelancer.Queries;

public record GetFreelancerByIdQuery(Guid Id, Guid UserId) : IRequest<Result<FreelancerDto>>;

public class GetFreelancerByIdHandler(IFreelancerRepository repo)
    : IRequestHandler<GetFreelancerByIdQuery, Result<FreelancerDto>>
{
    public async Task<Result<FreelancerDto>> Handle(GetFreelancerByIdQuery request, CancellationToken ct)
    {
        var freelancer = await repo.GetByIdAsync(request.Id);
        if (freelancer == null)
            return Result.Fail("Freelancer not found.");

        var dto = new FreelancerDto
        {
            Id = freelancer.Id,
            Username = freelancer.Username,
            Email = freelancer.Email,
            PhoneNumber = freelancer.PhoneNumber,
            Skills = freelancer.Skills.Select(s => s.SkillName).ToList(),
            Hobbies = freelancer.Hobbies.Select(h => h.HobbyName).ToList()
        };

        return Result.Ok(dto);
    }
}