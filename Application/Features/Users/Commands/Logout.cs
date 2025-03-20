using Application.Common;
using Application.Contracts.Authentication;
using MediatR;

namespace Application.Features.Users.Commands;

public record LogoutCommand(string UserId) : IRequest<ApiResponse<bool>>;

public class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, ApiResponse<bool>>
{
    private readonly IAuthenticationService _authenticationService;

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