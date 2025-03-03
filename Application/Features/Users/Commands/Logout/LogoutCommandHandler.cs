using Application.Common;
using Application.Contracts.Authentication;
using MediatR;

namespace Application.Features.Users.Commands.Logout;

public class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, ApiResponse<bool>>
{
    private IAuthenticationService _authenticationService;

    public LogoutCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _authenticationService.LogoutAsync(request.UserId);
        return ApiResponse<bool>.SuccessResult(true);
    }
}