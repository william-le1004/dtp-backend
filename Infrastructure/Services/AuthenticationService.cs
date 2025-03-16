using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Contracts.Caching;
using Application.Dtos;
using Domain.Entities;
using Infrastructure.Common.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IRedisCacheService _redisCache;
    private readonly IUserContextService _userContext;

    public AuthenticationService(UserManager<User> userManager, JwtTokenService jwtTokenService,
        IRedisCacheService redisCache, IUserContextService userContext)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _redisCache = redisCache;
        _userContext = userContext;
    }

    public async Task<bool> RegisterAsync(RegistrationRequestDto request)
    {
        var user = new User(request.UserName, request.Email, request.Name, request.Address, request.PhoneNumber);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new Exception($"User creation failed: {errorMessage}");
        }

        await _userManager.AddToRoleAsync(user, ApplicationRole.TOURIST);
        return true;
    }

    public async Task<AccessTokenResponse> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.UserNameOrPassword) ?? await _userManager.FindByNameAsync(request.UserNameOrPassword);
        if (user == null || !(await _userManager.CheckPasswordAsync(user, request.Password)))
            throw new UnauthorizedAccessException("Invalid username or password.");

        var tokens = await _jwtTokenService.GenerateTokens(user);
        await StoreRefreshToken(user.Id, tokens.RefreshToken);
        return tokens;
    }

    public async Task<AccessTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var userId = await _jwtTokenService.ValidateRefreshToken(refreshToken);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var storedToken = await _redisCache.GetDataAsync<string>($"{ApplicationConst.REFRESH_TOKEN}:{userId}");
        if (storedToken == null || storedToken != refreshToken)
            throw new UnauthorizedAccessException("Refresh token expired or invalid.");

        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new UnauthorizedAccessException("User not found.");
        
        await RevokeToken();

        var accessTokenResponse = await _jwtTokenService.GenerateTokens(user);
        await StoreRefreshToken(user.Id, accessTokenResponse.AccessToken);

        return accessTokenResponse;
    }

    private async Task RevokeToken()
    {
        var accessToken = _userContext.GetAccessToken();
        var tokenJti = _jwtTokenService.GetJtiFromToken(accessToken);
        var tokenExpiry = _jwtTokenService.GetTokenExpiry(accessToken);
        var expiryTime = tokenExpiry - DateTime.UtcNow;

        if (expiryTime > TimeSpan.Zero)
        {
            await _redisCache.SetDataAsync($"{ApplicationConst.BLACKLIST}:{tokenJti}", "revoked", expiryTime);
        }
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        await _redisCache.RemoveDataAsync($"{ApplicationConst.REFRESH_TOKEN}:{userId}");

        await RevokeToken();

        return true;
    }

    private async Task StoreRefreshToken(string userId, string refreshToken)
    {
        var refreshTokenKey = $"{ApplicationConst.REFRESH_TOKEN}:{userId}";
        await _redisCache.SetDataAsync(refreshTokenKey, refreshToken, TimeSpan.FromDays(7));
    }
}