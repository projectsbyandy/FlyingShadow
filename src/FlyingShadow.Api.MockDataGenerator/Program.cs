using Ardalis.GuardClauses;
using FlyingShadow.Api.MockDataGenerator.Handler;
using FlyingShadow.Api.MockDataGenerator.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddMockDataGenerator()
    .ProcessArguments(builder.Configuration);

using var host = builder.Build();

var mockDataHandler = Guard.Against.Null(host.Services.GetService<MockDataHandler>());

return await mockDataHandler.Process();