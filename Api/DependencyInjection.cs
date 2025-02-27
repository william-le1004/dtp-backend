using Application.Tour.Queries;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Api;


public static class DependencyInjection
{
    public static IServiceCollection AddEndpointServices(this IServiceCollection services)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<TourResponse>("Tours");

        services.AddControllers().AddOData(
            options => options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(
                routePrefix: "odata",
                model: modelBuilder.GetEdmModel()));
        return services;
    }
}