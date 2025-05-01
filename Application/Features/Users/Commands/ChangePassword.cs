using System.Net;
using Application.Common;
using Application.Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Commands;

public record ChangePasswordCommand(
    string OldPassword,
    string NewPassword) : IRequest<ApiResponse<bool>>;

public class ChangePasswordCommandHandler(
    UserManager<User> userManager,
    IUserContextService userContextService)
    : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = userContextService.GetCurrentUserId();
        var user = await userManager.FindByIdAsync(userId!);

        if (user is null)
            return ApiResponse<bool>.Failure("User not found");

        var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        return result.Succeeded
            ? ApiResponse<bool>.SuccessResult(true)
            : ApiResponse<bool>.Failure("Password change failed", (int)HttpStatusCode.BadRequest,
                result.Errors.Select(e => e.Description).ToList());
    }
}