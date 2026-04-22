using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.DTO.Configuration;

namespace FlyingShadow.Api.Integration.Tests.Fixtures;

public abstract class AuthenticationFixture
{
    protected async Task<string> GetAuthTokenAsync(HttpClient client, CancellationToken ct)
    {
        var user = Guard.Against.Null(ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers").LoginDetailsList).First();
        var authResponse = await client.PostAsJsonAsync($"/api/authentication/login", user, ct);
        var loginResponse = await authResponse.Content.ReadFromJsonAsync<LoginResponse>(ct);
        
        return Guard.Against.NullOrEmpty(loginResponse?.TokenDetails.Token);
    }

    protected void AddAuthHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}