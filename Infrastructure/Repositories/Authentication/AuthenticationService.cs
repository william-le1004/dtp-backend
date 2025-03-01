using Application.Common;
using Application.Contracts.Authentication;
using Application.Contracts.Caching;
using Application.Dtos;
using Domain.Entities;
using Infrastructure.Common.Constants;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories.Authentication;

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

    public async Task<ServiceResult<bool>> RegisterAsync(RegistrationRequestDto request)
    {
        var user = new User { Name = request.Name, Address = request.Address, Email = request.Email, UserName = request.UserName };

        var identityResult = await _userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            return ServiceResult<bool>.Failure(string.Join(", ",
                identityResult.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, ApplicationRole.TOURIST);
        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<AccessTokenResponse>> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user == null || !(await _userManager.CheckPasswordAsync(user, request.Password)))
            return ServiceResult<AccessTokenResponse>.Failure("Invalid username or password.");

        var tokens = await _jwtTokenService.GenerateTokens(user);
        await StoreRefreshToken(user.Id.ToString(), tokens.RefreshToken);
        return ServiceResult<AccessTokenResponse>.SuccessResult(tokens);
    }

    public async Task<ServiceResult<AccessTokenResponse>> RefreshTokenAsync(string refreshToken)
    {
        var userId = await _jwtTokenService.ValidateRefreshToken(refreshToken);
        if (string.IsNullOrEmpty(userId))
            return ServiceResult<AccessTokenResponse>.Failure("Invalid refresh token.");

        var storedToken = await _redisCache.GetDataAsync<string>($"refreshToken:{userId}");
        if (storedToken == null || storedToken != refreshToken)
            return ServiceResult<AccessTokenResponse>.Failure("Refresh token expired or invalid.");
        
        var user = await _userManager.FindByIdAsync(userId);
        var accessTokenResponse = await _jwtTokenService.GenerateTokens(user);

        await StoreRefreshToken(user.Id.ToString(), accessTokenResponse.AccessToken);
        return ServiceResult<AccessTokenResponse>.SuccessResult(accessTokenResponse);
    }

    public async Task<ServiceResult<bool>> LogoutAsync(string userId)
    {
        await _redisCache.RemoveDataAsync($"refreshToken:{userId}");
        return ServiceResult<bool>.SuccessResult(true); 
    }

    private async Task StoreRefreshToken(string userId, string refreshToken)
    {
        var refreshTokenKey = $"refreshToken:{userId}";
        await _redisCache.SetDataAsync(refreshTokenKey, refreshToken, TimeSpan.FromDays(7));
    }
}