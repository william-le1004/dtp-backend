using Application.Common;
using Application.Contracts.Persistence;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public record ConfirmEmailCommand(
    string ConfirmationToken)
    : IRequest<ApiResponse<bool>>;

public class ConfirmEmailHandler(UserManager<User> userManager, ILogger<ConfirmEmailHandler> logger, IUserRepository userRepository)
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
            return ApiResponse<bool>.Failure("Email confirmation failed", 400);
        }
        
        logger.LogInformation("User {Email} email confirmation result: {Result}", user.Email, result.Succeeded);
        return ApiResponse<bool>.SuccessResult(true);
    }
}