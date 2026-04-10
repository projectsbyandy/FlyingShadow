using System.Net;
using System.Net.Http.Json;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.Support.TestExtensions;
using FlyingShadow.Api.Integration.Tests.Support.TestLifeCycle;
using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Api.Integration.Tests;

[Collection(IntegrationTestCollection.Name)]
public class FlyingShadowTests
{
    private readonly HttpClient _client;
    
    public FlyingShadowTests(FlyingShadowWebAppTestFactory factory)
    {
        _client = factory.CreateClient();
    }
    
    [JsonMockDataFact]
    public async Task Verify_Get_Shadows_Returns_Correct_Data_With_Valid_Authentication_Token()
    {
        // Arrange
        var token = await GetAuthTokenAsync(_client);
        var jsonMockShadows = Guard.Against.Null(ConfigReader.GetConfigurationSection<List<Shadow>>("FakeShadows"));

        // Act
        _client.DefaultRequestHeaders.Add("authorization", $"bearer {token}");
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, shadowResponse.StatusCode);

        var shadowDetails = await shadowResponse.Content.ReadFromJsonAsync<IList<ShadowDto>>();
        
        Assert.Equal(jsonMockShadows.Count, shadowDetails?.Count);
    }
    
    [JsonMockDataFact]
    public async Task Verify_Get_Shadows_With_Invalid_Authentication_Token_Returns_Unauthorized()
    {
        // Arrange
        var token = await GetAuthTokenAsync(_client);
        var invalidToken = token + "test";
        
        // Act
        _client.DefaultRequestHeaders.Add("authorization", $"bearer {invalidToken}");
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, shadowResponse.StatusCode);
    }
    
    [JsonMockDataFact]
    public async Task Verify_Get_Shadows_With_A_Missing_Authentication_Token_Returns_Unauthorized()
    {
        // Arrange / Act
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, shadowResponse.StatusCode);
    }

    private async Task<string> GetAuthTokenAsync(HttpClient client)
    {
        var user = Guard.Against.Null(ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers").LoginDetailsList).First();
        var authResponse = await client.PostAsJsonAsync($"/api/authentication/login", user);
        var loginResponse = await authResponse.Content.ReadFromJsonAsync<LoginResponse>();
        
        return Guard.Against.NullOrEmpty(loginResponse?.TokenDetails.Token);
    }
}