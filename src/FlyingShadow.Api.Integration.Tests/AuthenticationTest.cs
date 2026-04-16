using System.Net;
using System.Net.Http.Json;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.DTO;
using FlyingShadow.Api.Integration.Tests.Support.TestExtensions;
using FlyingShadow.Api.Integration.Tests.Support.TestLifeCycle;
using FlyingShadow.Core.DTO.Authenticate;

namespace FlyingShadow.Api.Integration.Tests;

[Collection(IntegrationTestCollection.Name)]
public class AuthenticationTest
{
    private readonly HttpClient _client;
    private readonly FakeUsers _fakeUsers = ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers");
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
        
    public AuthenticationTest(FlyingShadowWebAppTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Login
    
    [JsonMockDataFact]
    public async Task Verify_Successful_Login_Returns_Token_and_Expiry()
    {
        // Arrange
        var firstValidUser = Guard.Against.Null(_fakeUsers.LoginDetailsList?.First());
        
        // Act
        var response = await _client.PostAsJsonAsync("api/authentication/login", firstValidUser, _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokenResponse = Guard.Against.Null(await response.Content.ReadFromJsonAsync<TokenResponse>());
        
        var expectedExpiry = DateTime.UtcNow.AddHours(1);
        Assert.True(tokenResponse.TokenDetails.ExpiresAt >= expectedExpiry.AddSeconds(-5));
        Assert.True(tokenResponse.TokenDetails.ExpiresAt <= expectedExpiry.AddSeconds(5));
        
        Assert.NotEqual(string.Empty, tokenResponse.TokenDetails.Token);
    }
    
    [JsonMockDataFact]
    public async Task Verify_No_Payload_Returns_UnsupportedMediaType()
    {
        // Arrange / Act
        var response = await _client.PostAsync("api/authentication/login", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
    
    [JsonMockDataTheory]
    [InlineData("", "")]
    [InlineData("bob", "test")]
    [InlineData("mary", "watch")]
    public async Task Verify_Invalid_Credentials_Returns_Unauthorized(string email, string password)
    {
        // Arrange / Act
        var response = await _client.PostAsJsonAsync("api/authentication/login", new LoginDetails()
        {
            Email = email,
            Password = password
        }, _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(new ErrorResponse("Invalid Email or Password"), await response.Content.ReadFromJsonAsync<ErrorResponse>(_cancellationToken));
    }
    
    [Fact]
    public async Task Verify_Missing_Email_Returns_Unauthorized()
    {
        // Arrange / Act
        var response = await _client.PostAsJsonAsync("api/authentication/login", new
        {
            Password = "password"
        }, _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseMessage = await response.Content.ReadAsStringAsync(_cancellationToken);
        Assert.Contains("missing required properties including: 'email'", responseMessage);
    }
    
    // NOTE other field validation is ignored as the mechanism is provided by the MS framework ModelStateInvalidFilter 
    #endregion

    #region Registration

    [JsonMockDataTheory]
    [InlineData("Paulie@traders.com", "89M+}^^7Tf34")]
    [InlineData("Phil@traders.com", "89hM+}^^7Tf2412")]
    public async Task Verify_Successful_Registration_Returns_User(string email, string password)
    {
        // Act
        var userToRegister = new
        {
            email,
            password
        };
        
        // Arrange
        var response = await _client.PostAsJsonAsync("api/authentication/register", userToRegister);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotEqual(Guid.Empty, registerResponse?.UserId);
    }
    
    [JsonMockDataFact]
    public async Task Verify_User_Cannot_Be_Registered_With_Existing_Registered_Email()
    {
        // Arrange
        var existingUser = Guard.Against.Null(_fakeUsers.LoginDetailsList?.First());
        
        // Act
        var response = await _client.PostAsJsonAsync("api/authentication/register", existingUser);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var registerResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal(new ErrorResponse("Registration could not be completed."), registerResponse);
    }
    
    #endregion
}