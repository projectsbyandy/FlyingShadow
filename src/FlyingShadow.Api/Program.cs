using System.Text;
using FlyingShadow.Api.Ioc;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.DTO.Configuration.MockData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFlyingShadowApiSupport()
    .AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info = new()
            {
                Title = "Flying Daggers API",
                Version = "1.0.0",
                Description = "Flying Daggers API management",
            };
            document.Servers =
            [
                new() { Url = "https://localhost:7113" }
            ];

            return Task.CompletedTask;
        });
    })
    .AddControllers();

var mockData = ConfigReader.GetConfigurationSection<MockData>("MockData");
if (mockData is { IsEnabled: true, Source: Source.Json })
{
    builder.Services.RegisterFakeJsonRepositories();
}

var jwtSettings = ConfigReader.GetConfigurationSection<Jwt>("Jwt");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key!)),
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapOpenApi("/openapi/FlyingShadow.Api.json");
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Flying Daggers API");
        options.WithOpenApiRoutePattern("/openapi/FlyingShadow.Api.json");

    });
}

if (!app.Environment.IsEnvironment("Test"))
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();