using FlyingShadow.Core.Utils;

namespace FlyingShadow.Api.Tests.Services;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _sut;
    private const string Password = "Hail Bob!";
    private const string TenRoundExpectedHash = "$2a$10$KZv9ilFXbBlE2YpGHE2TZuelIVIiGRPx0e7puw3fyeIhmyxpuj2oy";
    
    public PasswordHasherTests()
    {
        _sut = new PasswordHasher();
    }

    [Fact]
    public void Hash_ShouldReturnValidHash()
    {
        // Arrange / Act
        var hashedPassword = _sut.Hash(Password);
        
        // Assert
        Assert.NotNull(hashedPassword);
        Assert.NotEmpty(hashedPassword);
    }
    
    [Fact]
    public void Verify_WithValidDetails_ShouldReturnTrue()
    {
        // Arrange / Act
        var result = _sut.Verify(Password, TenRoundExpectedHash);
        
        // Assert
        Assert.True(result);
    }
}