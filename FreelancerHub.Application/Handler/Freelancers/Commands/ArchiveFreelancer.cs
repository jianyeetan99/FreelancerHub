using FluentResults;
using FreelancerHub.Application.Interfaces;
using MediatR;

namespace FreelancerHub.Application.Handler.Freelancer.Commands;

public record ArchiveFreelancerCommand(Guid Id, Guid UserId) : IRequest<Result>;

public class ArchiveFreelancerHandler(IFreelancerRepository repo) : IRequestHandler<ArchiveFreelancerCommand, Result>
{
    public async Task<Result> Handle(ArchiveFreelancerCommand request, CancellationToken cancellationToken)
    {
        var updated = await repo.ArchiveAsync(request.Id);
        return updated ? Result.Ok() : Result.Fail("Freelancer not found or already archived.");
    }
}