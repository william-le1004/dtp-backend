using Application.Features.Company.Queries;
using Application.Features.Tour.Queries;
using Application.Features.Users.Queries;
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
        modelBuilder.EntitySet<Category>("Category");
        modelBuilder.EntitySet<UserDto>("User");
        modelBuilder.EntitySet<CompanyDto>("Company");
        
        modelBuilder.EnableLowerCamelCase();
        services.AddControllers().AddOData(
            options => options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(
                routePrefix: "odata",
                model: modelBuilder.GetEdmModel()).Select().Filter().Count().OrderBy());

        return services;
    }
}