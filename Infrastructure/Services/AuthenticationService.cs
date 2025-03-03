using Application.Common;
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

    public AuthenticationService(UserManager<User> userManager, JwtTokenService jwtTokenService, IRedisCacheService redisCache)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _redisCache = redisCache;
    }

    public async Task<ApiResponse<bool>> RegisterAsync(RegistrationRequestDto request)
    {
        var user = new User { Name = request.Name, Address = request.Address, Email = request.Email, UserName = request.UserName, PhoneNumber = request.PhoneNumber };

        var identityResult = await _userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            return ApiResponse<bool>.Failure(string.Join(", ",
                identityResult.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, ApplicationRole.TOURIST);
        return ApiResponse<bool>.SuccessResult(true);
    }

    public async Task<ApiResponse<AccessTokenResponse>> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user == null || !(await _userManager.CheckPasswordAsync(user, request.Password)))
            return ApiResponse<AccessTokenResponse>.Failure("Invalid username or password.");

        var tokens = await _jwtTokenService.GenerateTokens(user);
        await StoreRefreshToken(user.Id.ToString(), tokens.RefreshToken);
        return ApiResponse<AccessTokenResponse>.SuccessResult(tokens);
    }

    public async Task<ApiResponse<AccessTokenResponse>> RefreshTokenAsync(string refreshToken)
    {
        var userId = await _jwtTokenService.ValidateRefreshToken(refreshToken);
        if (string.IsNullOrEmpty(userId))
            return ApiResponse<AccessTokenResponse>.Failure("Invalid refresh token.");

        var storedToken = await _redisCache.GetDataAsync<string>($"{ApplicationPrefix.REFRESH_TOKEN}:{userId}");
        if (storedToken == null || storedToken != refreshToken)
            return ApiResponse<AccessTokenResponse>.Failure("Refresh token expired or invalid.");
        
        var user = await _userManager.FindByIdAsync(userId);
        var accessTokenResponse = await _jwtTokenService.GenerateTokens(user);

        await StoreRefreshToken(user.Id.ToString(), accessTokenResponse.AccessToken);
        return ApiResponse<AccessTokenResponse>.SuccessResult(accessTokenResponse);
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string userId)
    {
        await _redisCache.RemoveDataAsync($"{ApplicationPrefix.REFRESH_TOKEN}:{userId}");
        return ApiResponse<bool>.SuccessResult(true); 
    }

    private async Task StoreRefreshToken(string userId, string refreshToken)
    {
        var refreshTokenKey = $"{ApplicationPrefix.REFRESH_TOKEN}:{userId}";
        await _redisCache.SetDataAsync(refreshTokenKey, refreshToken, TimeSpan.FromDays(7));
    }
}