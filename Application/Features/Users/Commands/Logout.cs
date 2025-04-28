using Application.Common;
using Application.Contracts.Authentication;
using MediatR;

namespace Application.Features.Users.Commands;

public record LogoutCommand(string UserId) : IRequest<ApiResponse<bool>>;

public class LogoutCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<LogoutCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await authenticationService.LogoutAsync(request.UserId);
        return ApiResponse<bool>.SuccessResult(true);
    }
}