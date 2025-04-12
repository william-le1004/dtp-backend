using System.Text;
using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Contracts.Caching;
using Application.Contracts.Cloudinary;
using Application.Contracts.EventBus;
using Application.Contracts.Job;
using Application.Contracts.Persistence;
using Domain.Constants;
using Domain.Entities;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Infrastructure.Common.Constants;
using Infrastructure.Common.Settings;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Repositories.MessageBroker;
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
        var redisSettings = environmentSection.GetSection("Redis");
        var cloudinarySettings = environmentSection.GetSection("CloudinarySettings");

        var jwtKey  = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
        
        services.Configure<JwtSettings>(jwtSettings);
        services.Configure<CloudinarySettings>(cloudinarySettings);
        
        services.AddDbContext<DtpDbContext>((sp, options) =>
        {
            options.UseMySQL(connectionString)
                .AddInterceptors(sp.GetService<AuditableEntityInterceptor>());
        });

        var redisConfig = new ConfigurationOptions
        {
            EndPoints = { redisSettings["Endpoint"] },
            Password = redisSettings["Password"],
            User = redisSettings["User"],
            AllowAdmin = true,
            ConnectTimeout = 10000,
            SyncTimeout = 10000
        };

        services.AddStackExchangeRedisCache(options => { options.ConfigurationOptions = redisConfig; });

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfig));

        services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<DtpDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAuthenticatorService, AuthenticatorService>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITourScheduleRepository, TourScheduleRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<JwtTokenService>();
        services.AddScoped<IDtpDbContext, DtpDbContext>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddSingleton<ICloudinaryService,CloudinaryService>();
        services.AddTransient<IEventBus, EventBus>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<IHangfireJobService, HangfireJobService>();
        services.AddScoped<IOrderJobService, OrderJobService>();
        services.AddScoped<IHangfireStorageService, HangfireStorageService>();

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
                IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(ApplicationConst.AuthenticatedUser, 
                policy => policy.RequireAuthenticatedUser());
            options.AddPolicy(ApplicationConst.AdminPermission, 
                policy => policy.RequireRole(ApplicationRole.ADMIN));
            options.AddPolicy(ApplicationConst.ManagementPermission,
                policy => policy.RequireRole(ApplicationRole.ADMIN, ApplicationRole.OPERATOR));
            options.AddPolicy(ApplicationConst.HighLevelPermission,
                policy => policy.RequireRole(ApplicationRole.ADMIN, ApplicationRole.OPERATOR, ApplicationRole.MANAGER));
        });

        services.AddCors(options =>
        {
            options.AddPolicy("all", corsPolicyBuilder => corsPolicyBuilder
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod());
        });
        
        services.AddHangfire(config => config
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseRedisStorage(ConnectionMultiplexer.Connect(redisConfig), new RedisStorageOptions
            {
                Prefix = ApplicationConst.HangfirePrefix,
                InvisibilityTimeout = TimeSpan.FromMinutes(30),
                FetchTimeout = TimeSpan.FromMinutes(5),
                Db = 0,
                ExpiryCheckInterval = TimeSpan.FromMinutes(30),
                DeletedListSize = 6,
                SucceededListSize = 6,
                UseTransactions = false
            }));
        
        services.AddHangfireServer();

        return services;
    }
}