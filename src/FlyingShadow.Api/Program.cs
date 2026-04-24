using System.Text;
using FlyingShadow.Api.Ioc;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.DTO.Configuration.MockData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFlyingShadowApiSupport()
    .AddOpenApi(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info = new()
            {
                Title = "Flying Daggers API",
                Version = "1.0.0",
                Description = "Flying Daggers API management",
            };
            
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
    app.MapOpenApi("/docs/FlyingShadow_OpenApiSpec.yaml");
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Flying Daggers API");
        options.WithOpenApiRoutePattern("/docs/FlyingShadow_OpenApiSpec.yaml");
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