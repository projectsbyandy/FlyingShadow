using Ardalis.GuardClauses;
using FlyingShadow.Api.MockDataGenerator.Handler;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;
using FlyingShadow.Api.MockDataGenerator.Handler.Internal;
using FlyingShadow.Api.MockDataGenerator.Utilities;
using FlyingShadow.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureServices(services =>
{
    services
        .AddSingleton<MockDataHandler>()
        .AddSingleton<IPreReqValidator, PreReqValidator>()
        .AddSingleton<ISecretGenerator, SecretGenerator>()
        .AddSingleton<IUserDataGenerator, UserDataGenerator>()
        .AddSingleton<IShadowDataCopy, ShadowDataCopy>()
        .AddSingleton<IFileManager, FileManager>()
        .AddSingleton<IPasswordHasher, PasswordHasher>();
});

var host = hostBuilder.Build();

var mockDataHandler = Guard.Against.Null(host.Services.GetService<MockDataHandler>());

return await mockDataHandler.Process(args);