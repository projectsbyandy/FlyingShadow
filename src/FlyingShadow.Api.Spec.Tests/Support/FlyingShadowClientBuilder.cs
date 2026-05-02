using FlyingShadow.Client;
using FlyingShadow.Client.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Ardalis.GuardClauses;

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

    private FlyingShadowClient BuildWithToken(string token)
    {
        var handlers = KiotaClientFactory.CreateDefaultHandlers();
        handlers.Add(new BearerTokenHandler(token));
        
        return new FlyingShadowClient(CreateAdapter(handlers));
    }

    private HttpClientRequestAdapter CreateAdapter(IList<DelegatingHandler> handlers)
    {
        var httpClient = KiotaClientFactory.Create(handlers, Server.CreateHandler());

        return new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient)
        {
            BaseUrl = Server.BaseAddress.ToString().TrimEnd('/'),
        };
    }
}