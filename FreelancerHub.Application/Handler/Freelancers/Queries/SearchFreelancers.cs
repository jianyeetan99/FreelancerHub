using FluentResults;
using FreelancerHub.Application.DTOs;
using FreelancerHub.Application.Interfaces;
using MediatR;

namespace FreelancerHub.Application.Handler.Freelancer.Queries;

public record SearchFreelancersQuery(string Keyword, Guid UserId) : IRequest<Result<List<FreelancerDto>>>;

public class SearchFreelancersHandler(IFreelancerRepository repo)
    : IRequestHandler<SearchFreelancersQuery, Result<List<FreelancerDto>>>
{
    public async Task<Result<List<FreelancerDto>>> Handle(SearchFreelancersQuery request, CancellationToken ct)
    {
        var freelancers = await repo.SearchAsync(request.Keyword);

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