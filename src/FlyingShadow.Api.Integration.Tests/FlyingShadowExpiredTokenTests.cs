using System.Net;
using System.Net.Http.Json;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.Fixtures;
using FlyingShadow.Api.Integration.Tests.Support;
using FlyingShadow.Api.Integration.Tests.Support.TestExtensions;
using FlyingShadow.Api.Integration.Tests.Support.TestLifeCycle;
using FlyingShadow.Core.DTO.Authenticate;

namespace FlyingShadow.Api.Integration.Tests;

[Collection(IntegrationTestCollection.Name)]
public class FlyingShadowIntegrationTests : AuthenticationFixture, IDisposable
{
    private readonly FlyingShadowWebAppTestFactory _factory;
    private readonly LoginDetails _loginDetails;
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
    
    public FlyingShadowIntegrationTests(FlyingShadowWebAppTestFactory factory)
    {
        _factory = factory;
        _loginDetails = Guard.Against.Null(ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers").LoginDetailsList).First();
    }
    
    [JsonMockDataFact]
    public async Task GetShadows_WithExpiredAuthenticationToken_ReturnsUnauthorized()
    {
        // Arrange
        TestConfigReader.Add("appsettings.test.json"); // expiry set to in the past

        using HttpClient client = _factory.CreateClient();
        var token = await GetAuthTokenAsync(client);
        
        // Act
        AddAuthHeader(client, token);
        var shadowResponse = await client.GetAsync("api/FlyingShadow/Shadows", _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, shadowResponse.StatusCode);
    }
    
    public void Dispose()
    {
        // Prevent pollution of test config settings into other test classes
        TestConfigReader.Reset();
    }
    
    private async Task<string> GetAuthTokenAsync(HttpClient client)
    {
        var authResponse = await client.PostAsJsonAsync($"/api/authentication/login", _loginDetails, _cancellationToken);
        var loginResponse = await authResponse.Content.ReadFromJsonAsync<LoginResponse>(_cancellationToken);
        
        return Guard.Against.NullOrEmpty(loginResponse?.TokenDetails.Token);
    }
}