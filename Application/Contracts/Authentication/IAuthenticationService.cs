using Application.Dtos;
using Domain.Entities;

namespace Application.Contracts.Authentication;

public interface IAuthenticationService
{
    Task<bool> RegisterAsync(RegistrationRequestDto request);
    Task<AccessTokenResponse> LoginAsync(LoginRequestDto request);
    Task<AccessTokenResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string userId);
    Task<string> GenerateConfirmUrl(string email, string confirmUrl);
}