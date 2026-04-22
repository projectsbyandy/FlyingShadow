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
public class FlyingShadowTests : AuthenticationFixture
{
    private readonly HttpClient _client;
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
    
    public FlyingShadowTests(FlyingShadowWebAppTestFactory factory)
    {
        _client = factory.CreateClient();
    }
    
    [JsonMockDataFact]
    public async Task Verify_Get_Shadows_Returns_Correct_Data_With_Valid_Authentication_Token()
    {
        // Arrange
        var token = await GetAuthTokenAsync(_client, _cancellationToken);
        var jsonMockShadows = Guard.Against.Null(ConfigReader.GetConfigurationSection<List<Shadow>>("FakeShadows"));

        // Act
        AddAuthHeader(_client, token);
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows",  _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, shadowResponse.StatusCode);

        var shadowDetails = await shadowResponse.Content.ReadFromJsonAsync<IList<ShadowDto>>(_cancellationToken);
        
        Assert.Equal(jsonMockShadows.Count, shadowDetails?.Count);
    }
    
    [JsonMockDataFact]
    public async Task Verify_Get_Shadows_With_Invalid_Authentication_Token_Returns_Unauthorized()
    {
        // Arrange
        const string invalidToken = "test";
        
        // Act
        AddAuthHeader(_client, invalidToken);
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows", _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, shadowResponse.StatusCode);
    }
    
    [JsonMockDataFact]
    public async Task Verify_Get_Shadows_With_A_Missing_Authentication_Token_Returns_Unauthorized()
    {
        // Arrange / Act
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows", _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, shadowResponse.StatusCode);
    }
}