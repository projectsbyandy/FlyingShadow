using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Repositories.Internal;
using FlyingShadow.Api.Services;
using FlyingShadow.Api.Services.Internal;
using FlyingShadow.Api.Utils;

namespace FlyingShadow.Api.Ioc;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlyingShadowApiSupport(this IServiceCollection services)
    {
        services.AddScoped<Configuration>(_ => ConfigReader.GetConfiguration<Configuration>())
            .AddScoped<IUserRepository, FakeUserRepository>()
            .AddScoped<IShadowRepository, FakeShadowRepository>()
            .AddScoped<IStealthMetricsRepository, FakeStealthMetricsRepository>()
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IShadowService, ShadowService>()
            .AddScoped<ITokenService, TokenService>();
        
        return services;
    }
}