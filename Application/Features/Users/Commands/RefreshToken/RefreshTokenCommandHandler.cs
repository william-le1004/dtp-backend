using Application.Common;
using Application.Contracts.Authentication;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, ApiResponse<AccessTokenResponse>>
{
    private IAuthenticationService _authenticationService;

    public RefreshTokenCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<ApiResponse<AccessTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var newToken = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
        if(!newToken.Success)
        {
            return ApiResponse<AccessTokenResponse>.Failure(newToken.Message, 400);
        }
        
        return ApiResponse<AccessTokenResponse>.SuccessResult(newToken.Data, "Refresh Token successfully");
    }
}