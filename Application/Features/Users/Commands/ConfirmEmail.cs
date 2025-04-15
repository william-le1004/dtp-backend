using System.Net;
using Application.Common;
using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Messaging;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public record ConfirmEmailCommand(
    string ConfirmationToken)
    : IRequest<ApiResponse<bool>>;

public class ConfirmEmailHandler(
    UserManager<User> userManager,
    ILogger<ConfirmEmailHandler> logger,
    IEventBus eventBus)
    : IRequestHandler<ConfirmEmailCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Where(u => u.SecureToken == request.ConfirmationToken)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (user == null)
        {
            logger.LogError("User with  email does not exist");
            return ApiResponse<bool>.Failure("User not found", 404);
        }
        var result = await userManager.ConfirmEmailAsync(user, request.ConfirmationToken);
        
        if (!result.Succeeded)
        {
            logger.LogError("Email confirmation failed, because {Error}", 
                result.Errors.Select(e => e.Description).ToList());
            return ApiResponse<bool>.Failure("Email confirmation failed");
        }

        await eventBus.PublishAsync(new UserAuthenticated(user.Name, user.UserName ?? "N/A", user.Email ?? "N/A"),
            cancellationToken);
        
        logger.LogInformation("User {Email} email confirmation result: {Result}", user.Email, result.Succeeded);
        return ApiResponse<bool>.SuccessResult(true);
    }
}