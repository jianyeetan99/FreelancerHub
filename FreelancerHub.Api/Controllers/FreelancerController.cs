using System.Security.Claims;
using FreelancerHub.Application.DTOs;
using FreelancerHub.Application.Handler.Freelancer.Commands;
using FreelancerHub.Application.Handler.Freelancer.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FreelancersController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllFreelancersQuery(GetUserId()));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors.Select(e => e.Message));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetFreelancerByIdQuery(id, GetUserId()));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors.Select(e => e.Message));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        var result = await mediator.Send(new SearchFreelancersQuery(keyword, GetUserId()));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors.Select(e => e.Message));
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateFreelancerDto dto)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new CreateFreelancerCommand(dto, userId));

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);

        return BadRequest(result.Errors.Select(e => e.Message));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateFreelancerDto dto)
    {
        var result = await mediator.Send(new UpdateFreelancerCommand(id, dto, GetUserId()));
        return result.IsSuccess ? NoContent() : NotFound(result.Errors.Select(e => e.Message));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteFreelancerCommand(id, GetUserId()));
        return result.IsSuccess ? NoContent() : NotFound(result.Errors.Select(e => e.Message));
    }

    [HttpPatch("{id}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var result = await mediator.Send(new ArchiveFreelancerCommand(id, GetUserId()));
        return result.IsSuccess ? Ok("Archived") : NotFound(result.Errors.Select(e => e.Message));
    }

    [HttpPatch("{id}/unarchive")]
    public async Task<IActionResult> Unarchive(Guid id)
    {
        var result = await mediator.Send(new UnarchiveFreelancerCommand(id, GetUserId()));
        return result.IsSuccess ? Ok("Unarchived") : NotFound(result.Errors.Select(e => e.Message));
    }
}