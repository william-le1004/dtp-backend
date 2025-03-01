using Application.Contracts.Authentication;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.Login;

public class LoginHandler(IAuthenticationService authenticationService)
    : IRequestHandler<LoginCommand, AccessTokenResponse>
{
    public async Task<AccessTokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = new LoginRequestDto(request.Email, request.Password);
        var userLogin = await authenticationService.LoginAsync(user);
        if(userLogin.Success == false)
        {
            return null;
        }
        
        var tokenResponse = new AccessTokenResponse
        {
            AccessToken = userLogin.Data.AccessToken,
            RefreshToken = userLogin.Data.RefreshToken,
            ExpiresIn = userLogin.Data.ExpiresIn
        };
        return tokenResponse;
    }
}