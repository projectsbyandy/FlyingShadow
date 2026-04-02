using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Services;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;

namespace FlyingShadow.Api.Ioc;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlyingShadowApiSupport(this IServiceCollection services)
    {
        services.AddScoped<Configuration>(_ => ConfigReader.GetConfiguration<Configuration>())
            .RegisterFakeRepositories()
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IShadowService, ShadowService>()
            .AddScoped<ITokenService, TokenService>();
        
        return services;
    }

    private static IServiceCollection RegisterFakeRepositories(this IServiceCollection services)
    {
        return services.AddSingleton<IUserRepository, FakeUserRepository>()
            .AddSingleton<IShadowRepository, FakeShadowRepository>()
            .AddSingleton<IStealthMetricsRepository, FakeStealthMetricsRepository>();
    }
}