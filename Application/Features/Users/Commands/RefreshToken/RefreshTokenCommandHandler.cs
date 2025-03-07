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
        try
        {
            var newToken = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
            return ApiResponse<AccessTokenResponse>.SuccessResult(newToken, "Refresh Token successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AccessTokenResponse>.Failure(ex.Message, 400);
        }
    }
}