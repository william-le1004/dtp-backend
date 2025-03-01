using System.Security.Claims;
using Application.Dtos;
using Application.Features.Users.Commands.Login;
using Application.Features.Users.Commands.Logout;
using Application.Features.Users.Commands.RefreshToken;
using Application.Features.Users.Commands.Registration;
using MediatR;

// using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationCommand request)
    {
        var response = await _mediator.Send(request);
        if (!response.Success) return BadRequest(response);
        
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var response = await _mediator.Send(new LogoutCommand(userId));

        if (!response.Success) return BadRequest(new { Message = response.Message });

        return Ok(new { Message = "Logged out successfully" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}