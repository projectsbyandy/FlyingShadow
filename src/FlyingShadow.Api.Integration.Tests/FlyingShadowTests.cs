using System.Net;
using System.Net.Http.Json;
using FlyingShadow.Api.Utils;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.Fixtures;
using FlyingShadow.Api.Integration.Tests.Support.TestExtensions;
using FlyingShadow.Api.Integration.Tests.Support.TestLifeCycle;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Api.Integration.Tests;

[Collection(IntegrationTestCollection.Name)]
public class FlyingShadowTests : IClassFixture<AuthenticationFixture>
{
    private readonly HttpClient _client;
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
    private readonly AuthenticationFixture _authFixture;
    
    public FlyingShadowTests(FlyingShadowWebAppTestFactory factory, AuthenticationFixture authFixture)
    {
        _authFixture = authFixture;
        _client = factory.CreateClient();
    }
    
    [JsonMockDataFact]
    public async Task GetShadows_WithAuthenticatedToken_ReturnsCorrectShadowDataCount()
    {
        // Arrange
        var token = await _authFixture.GetAuthTokenAsync(_client, _cancellationToken);
        var jsonMockShadows = Guard.Against.Null(ConfigReader.GetConfigurationSection<List<Shadow>>("FakeShadows"));

        // Act
        _authFixture.AddAuthHeader(_client, token);
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows",  _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, shadowResponse.StatusCode);

        var shadowDetails = await shadowResponse.Content.ReadFromJsonAsync<IList<ShadowDto>>(_cancellationToken);
        
        Assert.Equal(jsonMockShadows.Count, shadowDetails?.Count);
    }
    
    [JsonMockDataFact]
    public async Task GetShadows_WithInvalidAuthenticationToken_ReturnsUnauthorized()
    {
        // Arrange
        const string invalidToken = "test";
        
        // Act
        _authFixture.AddAuthHeader(_client, invalidToken);
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows", _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, shadowResponse.StatusCode);
    }
    
    [JsonMockDataFact]
    public async Task GetShadows_WithMissingAuthenticationToken_ReturnsUnauthorized()
    {
        // Arrange / Act
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows", _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, shadowResponse.StatusCode);
    }
}