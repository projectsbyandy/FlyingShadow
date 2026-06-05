using Ardalis.GuardClauses;
using FlyingShadow.Api.Db;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Services;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.Db;
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
        services.AddSingleton<Configuration>(_ => ConfigReader.GetConfiguration<Configuration>())
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IShadowService, ShadowService>()
            .AddScoped<IBattleService, BattleService>()
            .AddScoped<ITokenService, TokenService>()
            .AddSingleton<IPasswordHasher, PasswordHasher>()
            .AddSingleton<IShadowDtoMapper, ShadowDtoMapper>()
            .AddSingleton<IBattleProcessor, BattleProcessor>()
            .AddSingleton<IDbConnectionFactory>(sp =>
            {
                var dbServer = sp.GetRequiredService<Configuration>().DbServer;
                Guard.Against.Null(dbServer, "Error setting up DbConnectionFactory, DbServer configuration is not present");
                return new NpgSqlConnectionFactory(DbConnectionStringBuilder.Map(dbServer));
            })
            .AddTransient<IQueryProcessor, QueryProcessor>();
        
        return services;
    }
    
    public static IServiceCollection AddRepositories(this IServiceCollection services, bool isMock = false)
    {
        if (isMock)
        {
            services.AddSingleton<IUserRepository, FakeUserRepository>()
                .AddSingleton<IShadowRepository, FakeShadowRepository>()
                .AddSingleton<IStealthMetricsRepository, FakeStealthMetricsRepository>();
        }
        else
        {
            services
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IShadowRepository, ShadowRepository>()
                .AddScoped<IStealthMetricsRepository, StealthMetricsRepository>();
        }
        
        return services;
    }
}