using FlyingShadow.Client.Models;
using FlyingShadow.Api.Spec.Tests.Support;
using FlyingShadow.Client;

namespace FlyingShadow.Api.Spec.Tests;

[Collection(OpenApiSpecCollection.Name)]
public class AuthenticationTests
{
    private readonly FlyingShadowClient _client;
    private readonly FakeUserLoginsProvider _fakeUsers;
    
    public AuthenticationTests(FlyingShadowClientBuilder builder, FakeUserLoginsProvider fakeUsers)
    {
        _fakeUsers = fakeUsers;
        _client = builder.BuildUnauthenticated();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenResponse()
    {
        // Arrange
        var validUser = _fakeUsers.ValidUser();
        
        // Act
        var loginResponse = await _client.Api.Authentication.Login.PostAsync(
            new LoginDetails
            {
                Email = validUser.Email,
                Password = validUser.Password
            });
        
        // Assert
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.TokenDetails);
        Assert.False(string.IsNullOrEmpty(loginResponse.TokenDetails.Token));
        Assert.True(loginResponse.TokenDetails.ExpiresAt > DateTime.UtcNow);
    }
    
    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange / Act
        var exception = await Assert.ThrowsAsync<ProblemDetails>(() => _client.Api.Authentication.Login.PostAsync(
            new LoginDetails()
            {
                Email = "invalidEmail@test.com",
                Password = "invalidPassword",
            }));
        
        // Assert
        Assert.Equal(401, exception.ResponseStatusCode);
    }

    [Fact]
    public async Task Login_WithMissingPassword_ReturnsBadRequest()
    {
        // Arrange / Act
        var exception = await Assert.ThrowsAsync<ProblemDetails>(() => _client.Api.Authentication.Login.PostAsync(
            new LoginDetails()
            {
                Email = "test@test.com"
            }));
        
        // Assert
        Assert.Equal(400, exception.ResponseStatusCode);
    }

    [Fact]
    public async Task Register_WithNewUser_ReturnsUserId()
    {
        // Arrange / Act
        var registerResponse = await _client.Api.Authentication.Register.PostAsync(
            new RegisterRequest
            {
                Email = $"{Guid.NewGuid()}@test.com",
                Password = "testPassword"
            });

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotEqual(Guid.Empty, registerResponse.UserId);
    }

    [Fact]
    public async Task Register_WithNewUser_CanSubsequentlyLogin()
    {
        // Arrange
        var userDetails = new {
        
            Email = $"{Guid.NewGuid()}@test.com",
            Password = "testPassword",
        };

        await _client.Api.Authentication.Register.PostAsync(
            new RegisterRequest
            {
                Email = userDetails.Email,
                Password = userDetails.Password
            });
        
        // Act
        var loginResponse = await _client.Api.Authentication.Login.PostAsync(
            new LoginDetails
            {
                Email = userDetails.Email,
                Password = userDetails.Password
            }
        );

        // Assert
        Assert.NotNull(loginResponse?.TokenDetails);
        Assert.False(string.IsNullOrEmpty(loginResponse.TokenDetails.Token));
        Assert.True(loginResponse.TokenDetails.ExpiresAt > DateTime.UtcNow);
    }
    
    public static IEnumerable<object?[]> InvalidRegistrations => new[]
    {
        new object?[] { null, null },
        new object?[] { "", "" },
        new object?[] { null, "testPassword" },
        new object?[] { "test@test.com", null },
        new object?[] { "", "testPassword" },
        new object?[] { "test@test.com", "" },
        new object?[] { "not-an-email", "testPassword" },
        new object?[] { "test@test.com", "shor" }
    };
    
    [Theory]
    [MemberData(nameof(InvalidRegistrations))]
    public async Task Register_WithInvalidDetails_ReturnsBadRequest(string email, string password)
    {
        var details = new RegisterRequest
        {
            Email = email,
            Password = password,
        };
            
        // Arrange
        var exception = await Assert.ThrowsAsync<ProblemDetails>(() => _client.Api.Authentication.Register.PostAsync(details));
        
        // Assert
        Assert.Equal(400, exception.ResponseStatusCode);
    }
}