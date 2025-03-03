using Application.Common;
using Application.Dtos;

namespace Application.Contracts.Authentication;

public interface IAuthenticationService
{
    Task<ApiResponse<bool>> RegisterAsync(RegistrationRequestDto request);
    Task<ApiResponse<AccessTokenResponse>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<AccessTokenResponse>> RefreshTokenAsync(string refreshToken);
    Task<ApiResponse<bool>> LogoutAsync(string userId);
}