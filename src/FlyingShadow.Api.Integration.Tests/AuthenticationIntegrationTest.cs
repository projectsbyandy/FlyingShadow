using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.DTO;
using FlyingShadow.Api.Integration.Tests.Support;

namespace FlyingShadow.Api.Integration.Tests;

[Collection(IntegrationTestCollection.Name)]
public class AuthenticationIntegrationTest
{
    private readonly HttpClient _client;

    public AuthenticationIntegrationTest(FlyingShadowWebAppTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Verify_Successful_Login_Returns_Token()
    {
        // Arrange
        var fakeUsers = ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers");
        var firstValidUser = Guard.Against.Null(fakeUsers.LoginDetailsList?.First());
        
        // Act
        var response = await _client.PostAsJsonAsync("api/authentication/login", firstValidUser);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokenResponse = Guard.Against.Null(await response.Content.ReadFromJsonAsync<TokenResponse>());
        
        var expectedExpiry = DateTime.UtcNow.AddHours(1);
        Assert.True(tokenResponse.TokenDetails.ExpiresAt >= expectedExpiry.AddSeconds(-5));
        Assert.True(tokenResponse.TokenDetails.ExpiresAt <= expectedExpiry.AddSeconds(5));
        
        Assert.NotEqual(string.Empty, tokenResponse.TokenDetails.Token);
    }
    
    [Fact]
    public async Task Verify_No_Credentials_Returns_Token()
    {
        // Arrange
        
        
        // Arrange / Act
        var response = await _client.PostAsync("api/authentication/login", new StringContent(""));
        
        // Assert
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
}