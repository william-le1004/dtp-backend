using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Dtos;
using Domain.Entities;
using Infrastructure.Common.Constants;
using Infrastructure.Common.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class JwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;
    private readonly IDatabase _redisDb;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings, UserManager<User> userManager,
        IConnectionMultiplexer redis)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
        _redisDb = redis.GetDatabase();
    }
    
    public async Task<AccessTokenResponse> GenerateTokens(User user)
    {
        var accessToken = await GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        var userRole = await _userManager.GetRolesAsync(user);
        
        var response = new AccessTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            Role = userRole.FirstOrDefault() ?? "No role"
        };
        return response;
    }
    
    private async Task<string> GenerateJwtToken(User user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id),
            new Claim("company_id", user.CompanyId.ToString() ?? "None")
        };

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }
    
    public string GetJtiFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
    }

    public DateTime GetTokenExpiry(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.ValidTo;
    }
    
    public async Task<string> ValidateRefreshToken(string refreshToken)
    {
        var server = _redisDb.Multiplexer.GetServer(_redisDb.Multiplexer.GetEndPoints()[0]);
        var keys = server.Keys(pattern: $"{ApplicationConst.REFRESH_TOKEN}_:*");

        foreach (var key in keys)
        {
            if (await _redisDb.KeyTypeAsync(key) == RedisType.Hash)
            {
                var storedToken = await _redisDb.HashGetAsync(key, "data");
                string refreshTokenValue = storedToken.ToString().Trim('"');
                if (refreshTokenValue == refreshToken)
                    return key.ToString().Split(':')[1];
            }
        }

        return null;
    }
}