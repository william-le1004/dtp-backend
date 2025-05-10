using Application.Common;
using Application.Contracts;
using Application.Features.Users.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(
    IMediator mediator,
    IUserContextService userContextService,
    UserManager<User> userManager)
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

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpPut("store-token")]
    public async Task<IActionResult> StoreToken([FromBody] string fcmToken)
    {
        var userId = userContextService.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<bool>.Failure("User not authenticated", 401));
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound(ApiResponse<bool>.Failure("User not found", 404));
        }

        user.FcmToken = fcmToken;
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded
            ? Ok(ApiResponse<bool>.SuccessResult(true))
            : BadRequest(ApiResponse<bool>.Failure("Failed to update token", 400));
    }
}