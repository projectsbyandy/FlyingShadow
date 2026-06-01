using FlyingShadow.Client;
using FlyingShadow.Client.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Core.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FlyingShadow.Api.Spec.Tests.Support;

public sealed class FlyingShadowClientBuilder : WebApplicationFactory<Program>
{
    public FlyingShadowClient BuildUnauthenticated()
    {
        var handlers = KiotaClientFactory.CreateDefaultHandlers();

        return new FlyingShadowClient(CreateAdapter(handlers));
    }

    public async Task<FlyingShadowClient> BuildAuthenticatedAsync(LoginDetails details)
    {
        var client = BuildUnauthenticated();
        var loginResponse = await client.Api.Authentication.Login.PostAsync(details);
        Guard.Against.Null(loginResponse?.TokenDetails?.Token);
        
        return BuildWithToken(loginResponse.TokenDetails.Token);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IUserRepository>();
            services.RemoveAll<IShadowRepository>();
            services.RemoveAll<IStealthMetricsRepository>();

            services.AddSingleton<IUserRepository, FakeUserRepository>();
            services.AddSingleton<IShadowRepository, FakeShadowRepository>();
            services.AddSingleton<IStealthMetricsRepository, FakeStealthMetricsRepository>();
        });
    }
    
    private FlyingShadowClient BuildWithToken(string token)
    {
        var handlers = KiotaClientFactory.CreateDefaultHandlers();
        handlers.Add(new BearerTokenHandler(token));
        
        return new FlyingShadowClient(CreateAdapter(handlers));
    }

    private HttpClientRequestAdapter CreateAdapter(IList<DelegatingHandler> handlers)
    {
        DisableKiotaUpdateCheck();
        
        var httpClient = KiotaClientFactory.Create(handlers, Server.CreateHandler());

        return new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient)
        {
            BaseUrl = Server.BaseAddress.ToString().TrimEnd('/'),
        };
    }
    
    private static void DisableKiotaUpdateCheck() => Environment.SetEnvironmentVariable("KIOTA_OFFLINE_ENABLED", "true");
}