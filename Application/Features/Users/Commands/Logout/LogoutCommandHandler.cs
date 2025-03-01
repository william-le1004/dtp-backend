using Application.Common;
using Application.Contracts.Authentication;
using MediatR;

namespace Application.Features.Users.Commands.Logout;

public class LogoutCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<LogoutCommand, ServiceResult<bool>>
{
    public async Task<ServiceResult<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await authenticationService.LogoutAsync(request.UserId);
        return ServiceResult<bool>.SuccessResult(true);
    }
}