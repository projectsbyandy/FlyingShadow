using FlyingShadow.Api.Repositories;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Tests.Repositories;

public class FakeUserRepositoryTests
{
    private readonly IUserRepository _sut;
    
    public FakeUserRepositoryTests()
    {
        _sut = new FakeUserRepository();    
    }
    
    [Fact]
    public void AddUser_WithValidDetails_IsSuccessful()
    {
        // Arrange
        var user = new User()
        {
            UserId = Guid.NewGuid(),
            Email = "peter@test.com",
            HashedPassword = "$2a$14$EeavH1nADA.G/X.XluCm3ef.uxiW5CQCqk0nb/dq0R33s6l57AXxS"
        };
          
        // Act
        var result = _sut.AddUser(user);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Value.Email);
        Assert.Equal(user.HashedPassword, result.Value.HashedPassword);
        Assert.Equal(user.UserId, result.Value.UserId);
    }
    
    [Fact]
    public void GetUser_WithValidEmail_RetrievesUser()
    {
        // Arrange
        var user = new User()
        {
            UserId = Guid.NewGuid(),
            Email = "Roger@test.com"
        };
          
        _sut.AddUser(user);
        
        // Act
        var result = _sut.GetUser(user.Email);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result.Value);
    }
    
    [Theory]
    [InlineData("test@test.com")]
    [InlineData("larry@last.com")]
    public void GetUser_WithAnInvalidEmail_ReturnsError(string email)
    {
        // Arrange / Act
        var result = _sut.GetUser(email);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Error, new Error(ErrorCode.NotFound, $"User with {email} was not found"));
    }
    
    [Fact]
    public void EnsureUserDoesNotExist_WithNewEmail_ReturnsSuccess()
    {
        // Arrange
        const string userDoesNotExistEmail = "DoesNotExist@test.com";
        
        // Act
        var result = _sut.EnsureUserDoesNotExist(userDoesNotExistEmail);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.IsType<Outcome>(result.Value);
    }
    
    [Fact]
    public void EnsureUserDoesNotExist_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        const string userExistsEmail = "demo_user@sample.org";
        
        // Act
        var result = _sut.EnsureUserDoesNotExist(userExistsEmail);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(new Error(ErrorCode.AlreadyExists, $"User with {userExistsEmail} already registered"), result.Error);
    }
}