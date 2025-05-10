using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
        
        var mqConnection = configuration.GetSection("Environment:RabbitMQ");

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.UsingRabbitMq((ctx, cfg) =>
            {
                // cfg.Host(mqConnection["Host"], mqConnection["VirtualHost"], h =>
                // {
                //     h.Username(mqConnection["Username"]);
                //     h.Password(mqConnection["Password"]);
                // });
                
                cfg.Host("160.187.229.170", h =>
                {
                    h.Username("will-e");
                    h.Password("wille");
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
        
        return services;
    }
}