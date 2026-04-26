using FlyingShadow.Api.MockDataGenerator.Handler;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;
using FlyingShadow.Api.MockDataGenerator.Handler.Internal;
using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Utilities;
using FlyingShadow.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlyingShadow.Api.MockDataGenerator.Ioc;

internal static class ServiceExtensions
{
    public static IServiceCollection AddMockDataGenerator(this IServiceCollection service)
    {
        service
            .AddSingleton<MockDataHandler>()
            .AddSingleton<IPreReqValidator, PreReqValidator>()
            .AddSingleton<ISecretGenerator, SecretGenerator>()
            .AddSingleton<IUserDataGenerator, UserDataGenerator>()
            .AddSingleton<IShadowDataCopy, ShadowDataCopy>()
            .AddSingleton<IFileManager, FileManager>()
            .AddSingleton<IPasswordHasher, PasswordHasher>();
        
        return service;
    }

    public static IServiceCollection ProcessArguments(this IServiceCollection service, IConfiguration configuration)
    {
        service
            .AddOptions<MockDataOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return service;
    }
}