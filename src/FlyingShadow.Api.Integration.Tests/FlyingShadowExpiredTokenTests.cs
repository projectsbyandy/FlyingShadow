using System.Net;
using System.Net.Http.Json;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.Support;
using FlyingShadow.Api.Integration.Tests.Support.TestLifeCycle;
using FlyingShadow.Core.DTO.Authenticate;

namespace FlyingShadow.Api.Integration.Tests;

[Collection(IntegrationTestCollection.Name)]
public class FlyingShadowIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly LoginDetails _loginDetails;
    
    public FlyingShadowIntegrationTests(FlyingShadowWebAppTestFactory factory)
    {
        _loginDetails = Guard.Against.Null(ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers").LoginDetailsList).First();

        TestConfigReader.Add("appsettings.test.json");
        
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Verify_Get_Shadows_With_An_Expired_Authentication_Token_Returns_Unauthorized()
    {
        // Arrange
        var token = await GetAuthTokenAsync(_client);
        
        // Act
        _client.DefaultRequestHeaders.Add("authorization", $"bearer {token}");
        var shadowResponse = await _client.GetAsync("api/FlyingShadow/Shadows");
        
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
        var authResponse = await client.PostAsJsonAsync($"/api/authentication/login", _loginDetails);
        var loginResponse = await authResponse.Content.ReadFromJsonAsync<LoginResponse>();
        
        return Guard.Against.NullOrEmpty(loginResponse?.TokenDetails.Token);
    }
}