using FreelancerHub.Application.DTOs.Auth;
using FreelancerHub.Application.Handler.Auth.Commands;
using FreelancerHub.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await mediator.Send(new RegisterUserCommand(dto));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors.Select(e => e.Message));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await mediator.Send(new LoginUserCommand(dto));
        return result.IsSuccess ? Ok(new { token = result.Value }) : Unauthorized(result.Errors.Select(e => e.Message));
    }
}