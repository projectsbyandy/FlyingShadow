using System.Net;
using System.Net.Http.Json;
using FlyingShadow.Api.Utils;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.Fixtures;
using FlyingShadow.Api.Integration.Tests.Support;
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

        var shadowDetails = await shadowResponse.Content.ReadFromJsonAsync<IList<ShadowDto>>(TestJsonOptions.Default, _cancellationToken);
        
        Assert.Equal(jsonMockShadows.Count, shadowDetails?.Count);
    }

    [JsonMockDataFact]
    public async Task GetShadows_ReturnsShadow_WithCorrectDetails()
    {
        // Arrange
        var token = await _authFixture.GetAuthTokenAsync(_client, _cancellationToken);
        var jsonMockShadows = Guard.Against.Null(ConfigReader.GetConfigurationSection<List<Shadow>>("FakeShadows"));

        // Act
        _authFixture.AddAuthHeader(_client, token);
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows",  _cancellationToken);

        // Act
        var shadows = await shadowResponse.Content.ReadFromJsonAsync<IList<ShadowDto>>(TestJsonOptions.Default, _cancellationToken);
        Assert.NotNull(shadows);
        
        // Assert
        var shadow = shadows.ToList().Find(s => Guard.Against.Null(s.CodeName).Equals("Shadow Wolf II"));
        Assert.NotNull(shadow);
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-000000000031"), shadow.Id);
        Assert.Equal("Hidden Rain", shadow.Clan);
        Assert.Equal("Land of Lightning", shadow.Origin);
        Assert.Equal(Rank.Toshiyama, shadow.Rank);
        Assert.NotNull(shadow.ShadowSkills);
        Assert.Equal(AcrobaticsLevel.Beginner, shadow.ShadowSkills.AcrobaticsLevel);
        Assert.Equal(3667, shadow.ShadowSkills.InvisibilityDurationMs);
        Assert.Equal(53, shadow.ShadowSkills.ShadowBlendScore);
        Assert.Equal(28, shadow.ShadowSkills.SilenceRating);
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