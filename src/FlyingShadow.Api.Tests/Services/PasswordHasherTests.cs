using FlyingShadow.Core.Utils;

namespace FlyingShadow.Api.Tests.Services;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _sut;
    private const string Password = "Hail Bob!";
    private const string FourteenRoundExpectedHash = "$2a$14$n5yAsbuOBB9uU8wqZv4ibui1WkvdZYpDlQLYZ4DbeCFo6HRwcQGA2";
    
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
        Assert.NotEmpty(hashedPassword);    }
    
    [Fact]
    public void ValidateHash_WithValidDetails_ShouldReturnTrue()
    {
        // Arrange / Act
        var result = _sut.Verify(Password, FourteenRoundExpectedHash);
        
        // Assert
        Assert.True(result);
    }
}