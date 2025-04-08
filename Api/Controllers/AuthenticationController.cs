using Application.Common;
using Application.Contracts;
using Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IMediator mediator, IUserContextService userContextService) 
    : BaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var response = await mediator.Send(request);
        return HandleServiceResult(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationCommand request)
    {
        var response = await mediator.Send(request);
        return HandleServiceResult(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = userContextService.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<bool>.Failure("User not authenticated", 401));
        }

        var response = await mediator.Send(new LogoutCommand(userId));
        return HandleServiceResult(response);
    }

    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }
    
    [HttpPost("confirmation")]
    public async Task<IActionResult> EmailConfirm([FromBody] ConfirmEmailCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }
}