using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FlyingShadow.Api.Services;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.Models;

namespace FlyingShadow.Api.Tests.Services;

public class TokenServiceTests
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly string _email = "test@test.com";
    
    [Fact]
    public void GenerateTokenValid_WithValidJwtConfiguration_ReturnsValidToken()
    {
        // Arrange
        var configuration = new Configuration()
        {
            Jwt = new Jwt()
            {
                Key = "hr0mrFHNtG7pkXzchmeqkci2vWYD7Y2yAcBf02EDXmB",
                Issuer = "FlyingShadow App",
                Audience = "FlyingShadow.Api",
                ExpiryInHours = 1
            }
        };

        var tokenService = new TokenService(configuration);

        // Act
        var tokenResult = tokenService.GenerateToken(_id, _email);

        // Assert
        Assert.True(tokenResult.IsSuccess);
        Assert.NotNull(tokenResult.Value);
        var processedToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenResult.Value.Token);

        Assert.Equal(_id.ToString(), processedToken.Claims
            .First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        Assert.Equal(_email, processedToken.Claims
            .First(c => c.Type == ClaimTypes.Email).Value);

        Assert.True(Guid.TryParse(processedToken.Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Jti).Value, out _));
        
        Assert.Contains(configuration.Jwt.Audience, processedToken.Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Aud).Value);
        
        Assert.Contains(configuration.Jwt.Issuer, processedToken.Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Iss).Value);
        
        Assert.True(tokenResult.Value.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_WithInvalidJwtKey_ReturnsUnexpectedError()
    {
        // Arrange
        var configuration = new Configuration() {
            Jwt = new Jwt()
            {
                Key = "InvalidKey",
                Issuer = "FlyingShadow App",
                Audience = "FlyingShadow.Api",
            }};
        
        var tokenService = new TokenService(configuration);
        
        // Act
        var tokenResult = tokenService.GenerateToken(_id, _email);
        
        // Assert
        Assert.True(tokenResult.IsFailure);
        Assert.NotNull(tokenResult.Error);
        Assert.Equal(ErrorCode.UnexpectedError, tokenResult.Error.Code);
        Assert.NotEmpty(tokenResult.Error.Message);
    }
}