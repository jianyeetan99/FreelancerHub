using FluentResults;
using FreelancerHub.Application.Interfaces;
using MediatR;

namespace FreelancerHub.Application.Handler.Freelancer.Commands;

public record UnarchiveFreelancerCommand(Guid Id, Guid UserId) : IRequest<Result>;

public class UnarchiveFreelancerHandler(IFreelancerRepository repo)
    : IRequestHandler<UnarchiveFreelancerCommand, Result>
{
    public async Task<Result> Handle(UnarchiveFreelancerCommand request, CancellationToken cancellationToken)
    {
        var updated = await repo.UnarchiveAsync(request.Id);
        return updated ? Result.Ok() : Result.Fail("Freelancer not found or not archived.");
    }
}