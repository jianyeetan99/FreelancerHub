using FluentResults;
using FreelancerHub.Application.DTOs;
using FreelancerHub.Application.Interfaces;
using MediatR;

namespace FreelancerHub.Application.Handler.Freelancer.Queries;

public record GetAllFreelancersQuery(Guid UserId) : IRequest<Result<List<FreelancerDto>>>;

public class GetAllFreelancersHandler(IFreelancerRepository repo)
    : IRequestHandler<GetAllFreelancersQuery, Result<List<FreelancerDto>>>
{
    public async Task<Result<List<FreelancerDto>>> Handle(GetAllFreelancersQuery request, CancellationToken ct)
    {
        var freelancers = await repo.GetAllAsync();
        var dtos = freelancers.Select(f => new FreelancerDto
        {
            Id = f.Id,
            Username = f.Username,
            Email = f.Email,
            PhoneNumber = f.PhoneNumber,
            Skills = f.Skills.Select(s => s.SkillName).ToList(),
            Hobbies = f.Hobbies.Select(h => h.HobbyName).ToList()
        }).ToList();

        return Result.Ok(dtos);
    }
}