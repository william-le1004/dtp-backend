using Api.Extensions;
using Api.Filters;
using Application.Features.Company.Queries;
using Application.Features.Tour.Queries;
using Application.Features.Users.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Net.payOS;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddEndpointServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddControllers().AddOData(
            options => options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(
                routePrefix: "odata",
                model: GetEdmModel()).Select().Filter().Count().OrderBy());
        
        services.AddRedisOutputCache(options =>
        {
            options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(5)));
        });
        
        var payOs = new PayOS(configuration["Environment:PayOs:ClientId"] ?? throw new Exception("Cannot find environment"),
            configuration["Environment:PayOs:ApiKey"] ?? throw new Exception("Cannot find environment"),
            configuration["Environment:PayOs:ChecksumKey"] ?? throw new Exception("Cannot find environment"));
        services.AddSingleton(payOs);
        services.AddScoped<OtpAuthorizeFilter>();
        
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
        
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        return services;
    }
    
    private static IEdmModel GetEdmModel()
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<TourTemplateResponse>("Tour");
        modelBuilder.EntityType<TourScheduleResponse>()
            .Property(x => x.OpenDate).AsDate();
        modelBuilder.EntitySet<Destination>("Destination");
        modelBuilder.EntitySet<Category>("Category");
        modelBuilder.EntitySet<UserDto>("User");
        modelBuilder.EntitySet<CompanyDto>("Company");
        modelBuilder.EnableLowerCamelCase();
        return modelBuilder.GetEdmModel();
    }
}