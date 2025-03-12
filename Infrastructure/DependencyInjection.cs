using System.Text;
using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Domain.Entities;
using Infrastructure.Common.Constants;
using Infrastructure.Common.Settings;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services,
        IConfiguration configuration)
    {
        var environmentSection = configuration.GetSection("Environment");

        var connectionString = environmentSection.GetConnectionString("DefaultConnection");
        var jwtSettings = environmentSection.GetSection("JwtSettings");
        var redisConnection = environmentSection.GetSection("Redis");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        services.Configure<JwtSettings>(jwtSettings);

        services.AddDbContext<DtpDbContext>((sp, options) =>
        {
            options.UseMySQL(connectionString)
                .AddInterceptors(sp.GetService<AuditableEntityInterceptor>());
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection["Endpoint"];
            options.ConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { redisConnection["Endpoint"] },
                Password = redisConnection["Password"],
                ConnectTimeout = 10000,
                SyncTimeout = 10000
            };
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnection["Endpoint"], true);
            configuration.Password = redisConnection["Password"];
            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<DtpDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<JwtTokenService>();
        services.AddScoped<IDtpDbContext, DtpDbContext>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextService, UserContextService>();

        services.AddAuthentication(item =>
        {
            item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(item =>
        {
            item.RequireHttpsMetadata = true;
            item.SaveToken = true;
            item.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = jwtSettings["Audience"],
                ValidIssuer = jwtSettings["Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(ApplicationConst.AUTH_POLICY, policy => policy.RequireAuthenticatedUser());
            options.AddPolicy(ApplicationConst.ADMIN_POLICY, policy => policy.RequireRole(ApplicationRole.ADMIN));
            options.AddPolicy(ApplicationConst.OPERATOR_POLICY,
                policy => policy.RequireRole(ApplicationRole.OPERATOR));
            options.AddPolicy(ApplicationConst.AD_OR_OP_POLICY,
                policy => policy.RequireRole(ApplicationRole.ADMIN, ApplicationRole.OPERATOR));
        });

        services.AddCors(options =>
        {
            options.AddPolicy("all", corsPolicyBuilder => corsPolicyBuilder
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod());
        });

        return services;
    }
}