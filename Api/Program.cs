using Api;
using Api.Middlewares;
using Application;
using Infrastructure;
using Infrastructure.Common.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables (for local development)
DotNetEnv.Env.Load();

// Read values from environment
var dbUser = Environment.GetEnvironmentVariable("MYSQL_USER");
var dbPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("MYSQL_DATABASE");

var configuration = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddInfrastructureService(configuration)
    .AddApplicationServices()
    .AddEndpointServices();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors(options =>
{
    options.AddPolicy("all", corsPolicyBuilder => corsPolicyBuilder
        .AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod());
});

builder.Services.AddSwaggerGen(c =>
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

var app = builder.Build();

app.UseMiddleware<JwtBlacklistMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.ApplyMigrations();
}

app.UseCors("all");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();