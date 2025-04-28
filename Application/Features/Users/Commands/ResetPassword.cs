using System.Net;
using Application.Common;
using Application.Contracts.Authentication;
using Application.Contracts.EventBus;
using Application.Messaging;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public record ResetPasswordCommand(
    string ConfirmationToken,
    string NewPassword)
    : IRequest<ApiResponse<bool>>;

public class ResetPasswordHandler(
    UserManager<User> userManager,
    ILogger<ResetPasswordHandler> logger)
    : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Where(u => u.SecureToken == request.ConfirmationToken)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (user == null)
        {
            logger.LogError("User with email does not exist");
            return ApiResponse<bool>.Failure("User not found", 404);
        }

        var result = await userManager.ResetPasswordAsync(user, request.ConfirmationToken, request.NewPassword);
        
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            logger.LogError("Reset Password failed, because {Error}",
                errors);
            return ApiResponse<bool>.Failure("Reset Password failed", (int)HttpStatusCode.BadRequest, errors);
        }
        
        logger.LogInformation("Reset Password Successfully");
        return ApiResponse<bool>.SuccessResult(true);
    }
}