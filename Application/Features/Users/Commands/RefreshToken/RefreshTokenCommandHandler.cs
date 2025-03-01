using Application.Contracts.Authentication;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler (IAuthenticationService authenticationService)
    : IRequestHandler<RefreshTokenCommand, AccessTokenResponse>
{
    public async Task<AccessTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var newToken = await authenticationService.RefreshTokenAsync(request.RefreshToken);
        if(newToken == null)
        {
            return null;
        }
        var tokenResponse = new AccessTokenResponse
        {
            AccessToken = newToken.Data.AccessToken,
            RefreshToken = newToken.Data.RefreshToken,
            ExpiresIn = newToken.Data.ExpiresIn
        }; 
        return tokenResponse;
    }
}