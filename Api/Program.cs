using Api;
using Api.Middlewares;
using Application;
using Infrastructure;
using Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables (for local development)
DotNetEnv.Env.Load();


var configuration = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddInfrastructureService(configuration)
    .AddApplicationServices(configuration)
    .AddEndpointServices(configuration);
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

app.UseMiddleware<JwtBlacklistMiddleware>();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
// app.ApplyMigrations();

app.UseCors("all");
app.UseAuthentication();
app.UseAuthorization();
app.UseODataRouteDebug();
app.UseOutputCache();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();