using FlyingShadow.Api.MockDataGenerator.Handler.Generate;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;

namespace FlyingShadow.Api.MockDataGenerator.Tests.Handler.Generate;

public class SecretGeneratorTests
{
    private readonly ISecretGenerator _sut;

    public SecretGeneratorTests()
    {
        _sut = new SecretGenerator();
    }

    [Fact]
    public void Jwt_Generates_128LengthSecret()
    {
        // Arrange / Act
        var secret = _sut.Jwt();
        
        // Assert
        Assert.NotNull(secret);
        Assert.Equal(128, secret.Length);
    }

    [Fact]
    public void Password_WithNoLength_GeneratesDefaultLength16()
    {
        // Arrange / Act
        var password = _sut.Password();
        
        // Assert
        Assert.Equal(16, password.Length);
    }
    
    [Theory]
    [InlineData(10)]
    [InlineData(171)]
    [InlineData(257)]
    [InlineData(500)]
    public void Password_WithLengthSpecified_GeneratesPasswordToLength(int length)
    {
        // Arrange / Act
        var password = _sut.Password(length);
        
        // Assert
        Assert.Equal(length, password.Length);
    }
    
    [Fact]
    public void Password_OnlyContainsSpecificCharacters()
    {
        // Arrange
        var validCharacters = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@#$".ToCharArray();
        
        // Act
        var password = _sut.Password();
        var characters = password.ToCharArray();
        
        // Assert
        characters.ToList().ForEach(c => Assert.Contains(c, validCharacters));
    }

    [Fact]
    public void Password_AreUniqueOnEachCall()
    {
        Assert.NotEqual(_sut.Password(), _sut.Password());
    }
    
    [Theory]
    [InlineData(9)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Password_LessThan10_ThrowsArgumentOutOfRangeException(int length)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(()=> _sut.Password(length));
        Assert.Equal("length", exception.ParamName);
        Assert.Contains("must be greater than or equal to 10", exception.Message);
    }
}