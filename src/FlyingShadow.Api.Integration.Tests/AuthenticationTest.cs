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
    [InlineData("test@test.com", "boring")]
    [InlineData("bob@test.com", "tester")]
    [InlineData("mary@test.com", "watch")]
    public async Task Verify_Login_With_Invalid_Credentials_Returns_Unauthorized(string email, string password)
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
    public async Task Verify_Login_Missing_Email_Returns_Unauthorized()
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
        var response = await _client.PostAsJsonAsync("api/authentication/register", userToRegister,  _cancellationToken);
        
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
        var response = await _client.PostAsJsonAsync("api/authentication/register", existingUser,  _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var registerResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal(new ErrorResponse("Registration could not be completed."), registerResponse);
    }
    
    public static IEnumerable<object?[]> InvalidRegistrations => new[]
    {
        new object?[] { null, null, new [] {"The Email field is required.", "The Password field is required." } },
        new object?[] { "", "", new [] { "The Email field is not a valid e-mail address.", "The field Password must be a string or array type with a minimum length of '5'." } },
        new object?[] { null, "testPassword", new [] { "The Email field is required." } },
        new object?[] { "test@test.com", null, new[] { "The Password field is required." } },
        new object?[] { "", "testPassword", new[] { "The Email field is not a valid e-mail address." } },
        new object?[] { "test@test.com", "",  new[] { "The field Password must be a string or array type with a minimum length of '5'." } },
        new object?[] { "not-an-email", "testPassword", new[] { "The Email field is not a valid e-mail address." } },
        new object?[] { "test@test.com", "shor", new[] { "The field Password must be a string or array type with a minimum length of '5'." }}
    };
    
    [Theory]
    [MemberData(nameof(InvalidRegistrations))]
    public async Task Verify_Register_With_Invalid_Details_Returns_Correct_Validation_Response(string email, string password, string[] expectedMessages)
    {
        // Arrange
        var existingUser = new RegisterRequest()
        {
            Email = email,
            Password = password
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("api/authentication/register", existingUser,  _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseText = await response.Content.ReadAsStringAsync(_cancellationToken);

        foreach (var message in expectedMessages)
        {
            Assert.Contains(message, responseText);

        }
    }
    
    #endregion
}