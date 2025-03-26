using Application.Contracts;
using Application.Contracts.Caching;
using Domain.Constants;
using Infrastructure.Common.Constants;
using Infrastructure.Services;

namespace Api.Middlewares;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public JwtBlacklistMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var userContext = scope.ServiceProvider.GetRequiredService<IUserContextService>();
            var redisCache = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();
            var jwtTokenService = scope.ServiceProvider.GetRequiredService<JwtTokenService>();

            var token = userContext.GetAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                var tokenJti = jwtTokenService.GetJtiFromToken(token);
                var isBlacklisted = await redisCache.GetDataAsync<string>($"{ApplicationConst.BlacklistPrefix}:{tokenJti}");

                if (!string.IsNullOrEmpty(isBlacklisted))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token has been revoked.");
                    return;
                }
            }
        }

        await _next(context);
    }
}