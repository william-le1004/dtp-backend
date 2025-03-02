using System.Text;
using Application.Contracts.Persistence;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services,
        IConfiguration configuration)
    {
        var environmentSection = configuration.GetSection("Environment");

        var connectionString = environmentSection.GetConnectionString("DefaultConnection");
        var jwtSettings = environmentSection.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        services.AddDbContext<DtpDbContext>(options => { options.UseMySQL(connectionString); });

        services.AddScoped<IDtpDbContext, DtpDbContext>();

        //
        // services.AddDbContext<DtpAuthDbContext>(options =>
        // {
        //     options.UseMySQL(connectionString);
        // });
        //
        // services.AddIdentity<User, IdentityRole>()
        //     .AddEntityFrameworkStores<DtpAuthDbContext>()
        //     .AddDefaultTokenProviders();

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
        return services;
    }
}