using System.Reflection;
using Application;
using Application.Tour.Queries;
using Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddInfrastructureService(configuration).AddApplicationServices();


var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<TourResponse>("Tours");

builder.Services.AddControllers().AddOData(
    options => options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(
        routePrefix: "odata",
        model: modelBuilder.GetEdmModel()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();