using FlyingShadow.Client;
using FlyingShadow.Client.Models;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Ardalis.GuardClauses;

namespace FlyingShadow.Api.Spec.Tests;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly FlyingShadowClient _client;
    private readonly FakeUsers _fakeUsers = Guard.Against.Null(ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers"));

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        var httpClient = factory.CreateClient();
        var adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient);
        _client = new FlyingShadowClient(adapter);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenResponse()
    {
        // Arrange
        var validUser = Guard.Against.Null(_fakeUsers.LoginDetailsList).First();
        
        // Act
        var loginResponse = await _client.Api.Authentication.Login.PostAsync(
            new LoginDetails()
            {
                Email = validUser.Email,
                Password = validUser.Password
            });
        
        // Assert
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.TokenDetails);
        Assert.NotEqual(string.Empty, loginResponse.TokenDetails.Token);
        Assert.True(loginResponse.TokenDetails.ExpiresAt > DateTime.UtcNow);
    }
    
    [Fact]
    public async Task Login_WithinInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange / Act
        var exception = await Assert.ThrowsAsync<ProblemDetails>(() => _client.Api.Authentication.Login.PostAsync(
            new LoginDetails()
            {
                Email = "invalidEmail",
                Password = "invalidPassword",
            }));
        
        // Assert
        Assert.Equal(401, exception.ResponseStatusCode);
    }
}