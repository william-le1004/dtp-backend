using Application.Common;
using Application.Dtos;

namespace Application.Contracts.Authentication;

public interface IAuthenticationService
{
    Task<ServiceResult<bool>> RegisterAsync(RegistrationRequestDto request);
    Task<ServiceResult<AccessTokenResponse>> LoginAsync(LoginRequestDto request);
    Task<ServiceResult<AccessTokenResponse>> RefreshTokenAsync(string refreshToken);
    Task<ServiceResult<bool>> LogoutAsync(string userId);
}