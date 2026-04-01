using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FlyingShadow.Api.Services;
using FlyingShadow.Core.DTO.Configuration;

namespace FlyingShadow.Api.Tests.Services;

public class TokenServiceTests
{
    [Fact]
    public void Verify_Valid_JWT_Token_Generated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "test@test.com";

        var configuration = new Configuration()
        {
            Jwt = new Jwt()
            {
                Key = "hr0mrFHNtG7pkXzchmeqkci2vWYD7Y2yAcBf02EDXmB",
                Issuer = "FlyingShadow App",
                Audience = "FlyingShadow.Api",
            }
        };

        var tokenService = new TokenService(configuration);

        // Act
        var tokenResult = tokenService.GenerateToken(id, email);

        // Assert
        Assert.True(tokenResult.IsSuccess);

        var processedToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenResult.Value);

        Assert.Equal(id.ToString(), processedToken.Claims
            .First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        Assert.Equal(email, processedToken.Claims
            .First(c => c.Type == ClaimTypes.Email).Value);

        Assert.True(Guid.TryParse(processedToken.Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Jti).Value, out _));
        
        Assert.Contains(configuration.Jwt.Audience, processedToken.Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Aud).Value);
        
        Assert.Contains(configuration.Jwt.Issuer, processedToken.Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Iss).Value);
    }

    [Fact]
    public void Verify_Error_When_Invalid_JWT_Key()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "test@test.com";
        
        var configuration = new Configuration() {
            Jwt = new Jwt()
            {
                Key = "InvalidKey",
                Issuer = "FlyingShadow App",
                Audience = "FlyingShadow.Api",
            }};
        
        var tokenService = new TokenService(configuration);
        
        // Act
        var tokenResult = tokenService.GenerateToken(id, email);
        
        // Assert
        Assert.False(tokenResult.IsSuccess);
        Assert.NotNull(tokenResult.Error);
        Assert.Equal("TOKEN_GENERATION_ERROR", tokenResult.Error.Code);
        Assert.NotEmpty(tokenResult.Error.Message);
    }
}