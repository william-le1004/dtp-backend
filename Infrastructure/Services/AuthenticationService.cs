using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Contracts.Caching;
using Application.Dtos;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AuthenticationService(
    UserManager<User> userManager,
    JwtTokenService jwtTokenService,
    IRedisCacheService redisCache,
    IUserContextService userContext)
    : IAuthenticationService
{
    public async Task<bool> RegisterAsync(RegistrationRequestDto request)
    {
        var user = new User(request.UserName, request.Email, request.Name, request.Address, request.PhoneNumber);

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errorMessage}");
        }

        await userManager.AddToRoleAsync(user, ApplicationRole.TOURIST);
        return true;
    }

    public async Task<AccessTokenResponse> LoginAsync(LoginRequestDto request)
    {
        var user = await userManager.FindByNameAsync(request.UserNameOrPassword) ??
                   await userManager.FindByEmailAsync(request.UserNameOrPassword);
        if (user == null || !(await userManager.CheckPasswordAsync(user, request.Password)))
            throw new UnauthorizedAccessException("Invalid username, email or password.");

        if (!user.EmailConfirmed)
            throw new UnauthorizedAccessException("Email is not confirmed.");

        var tokens = await jwtTokenService.GenerateTokens(user);
        await StoreRefreshToken(user.Id, tokens.RefreshToken);
        return tokens;
    }

    public async Task<AccessTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var userId = await jwtTokenService.ValidateRefreshToken(refreshToken);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var storedToken = await redisCache.GetDataAsync<string>($"{ApplicationConst.RefreshTokenPrefix}:{userId}");
        if (storedToken == null || storedToken != refreshToken)
            throw new UnauthorizedAccessException("Refresh token expired or invalid.");

        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new UnauthorizedAccessException("User not found.");
        
        await RevokeToken();

        var accessTokenResponse = await jwtTokenService.GenerateTokens(user);
        await StoreRefreshToken(user.Id, accessTokenResponse.RefreshToken);

        return accessTokenResponse;
    }

    private async Task RevokeToken()
    {
        var accessToken = userContext.GetAccessToken();
        var tokenJti = jwtTokenService.GetJtiFromToken(accessToken);
        var tokenExpiry = jwtTokenService.GetTokenExpiry(accessToken);
        var expiryTime = tokenExpiry - DateTime.UtcNow;

        if (expiryTime > TimeSpan.Zero)
        {
            await redisCache.SetDataAsync($"{ApplicationConst.BlacklistPrefix}:{tokenJti}", "revoked", expiryTime);
        }
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        await redisCache.RemoveDataAsync($"{ApplicationConst.RefreshTokenPrefix}:{userId}");

        await RevokeToken();

        return true;
    }

    public async Task<string> GenerateConfirmUrl(string email, string confirmUrl)
    {
        var user = await GetUserByEmail(email);

        var token = await GenerateSecureToken(user);

        var emailConfirmationUrl = $"{confirmUrl}?confirmationToken={token}";
        return emailConfirmationUrl;
    }

    private async Task<string> GenerateSecureToken(User? user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        user.SecureToken = token;
        await userManager.UpdateAsync(user);
        return token;
    }

    private async Task<User?> GetUserByEmail(string userName) =>
            await userManager.Users
                .Where(x => x.Email == userName)
                .FirstOrDefaultAsync();

    private async Task StoreRefreshToken(string userId, string refreshToken)
    {
        var refreshTokenKey = $"{ApplicationConst.RefreshTokenPrefix}:{userId}";
        await redisCache.SetDataAsync(refreshTokenKey, refreshToken, TimeSpan.FromDays(7));
    }
}