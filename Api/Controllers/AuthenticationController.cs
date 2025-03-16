using Application.Common;
using Application.Contracts;
using Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IUserContextService _userContextService;

    public AuthenticationController(IMediator mediator, IUserContextService userContextService)
    {
        _mediator = mediator;
        _userContextService = userContextService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var response = await _mediator.Send(request);
        return HandleServiceResult(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationCommand request)
    {
        var response = await _mediator.Send(request);
        return HandleServiceResult(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = _userContextService.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<bool>.Failure("User not authenticated", 401));
        }

        var response = await _mediator.Send(new LogoutCommand(userId));
        return HandleServiceResult(response);
    }

    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var response = await _mediator.Send(command);
        return HandleServiceResult(response);
    }
}