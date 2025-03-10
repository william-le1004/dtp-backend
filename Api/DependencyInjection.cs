using Application.Features.Tour.Queries;
using Application.Features.Users.Queries.Get;
using Domain.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddEndpointServices(this IServiceCollection services)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<TourTemplateResponse>("Tour");
        modelBuilder.EntitySet<Destination>("Destination");
        modelBuilder.EntitySet<UserDto>("User");

        services.AddControllers().AddOData(
            options => options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(
                routePrefix: "odata",
                model: modelBuilder.GetEdmModel()));
        return services;
    }
}