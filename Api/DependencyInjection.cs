using Application.Tour.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Api;


public static class DependencyInjection
{
    public static IServiceCollection AddEndpointServices(this IServiceCollection services)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<TourResponse>("Tours");
        modelBuilder.EntitySet<Destination>("Destinations");

        services.AddControllers().AddOData(
            options => options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(
                routePrefix: "odata",
                model: modelBuilder.GetEdmModel()));
        return services;
    }
}