using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Services;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;
using FlyingShadow.Core.Services.Battle;
using FlyingShadow.Core.Services.Mappers;
using FlyingShadow.Core.Utils;

namespace FlyingShadow.Api.Ioc;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlyingShadowApiSupport(this IServiceCollection services)
    {
        services.AddScoped<Configuration>(_ => ConfigReader.GetConfiguration<Configuration>())
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddScoped<IShadowService, ShadowService>()
            .AddScoped<IBattleService, BattleService>()
            .AddScoped<ITokenService, TokenService>()
            .AddSingleton<IShadowDtoMapper, ShadowDtoMapper>()
            .AddSingleton<IBattleProcessor, BattleProcessor>();
        
        return services;
    }

    public static IServiceCollection RegisterFakeJsonRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, FakeUserRepository>()
            .AddSingleton<IShadowRepository, FakeShadowRepository>()
            .AddSingleton<IStealthMetricsRepository, FakeStealthMetricsRepository>();
        
        return services;
    }
}