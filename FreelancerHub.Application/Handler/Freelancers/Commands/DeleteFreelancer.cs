using FluentResults;
using FreelancerHub.Application.Interfaces;
using MediatR;

namespace FreelancerHub.Application.Handler.Freelancer.Commands;

public record DeleteFreelancerCommand(Guid Id, Guid UserId) : IRequest<Result>;

public class DeleteFreelancerHandler(IFreelancerRepository repo) : IRequestHandler<DeleteFreelancerCommand, Result>
{
    public async Task<Result> Handle(DeleteFreelancerCommand request, CancellationToken ct)
    {
        var existing = await repo.GetByIdAsync(request.Id);
        if (existing == null)
            return Result.Fail("Freelancer not found.");

        var deleted = await repo.DeleteAsync(request.Id);
        return deleted ? Result.Ok() : Result.Fail("Delete failed.");
    }
}