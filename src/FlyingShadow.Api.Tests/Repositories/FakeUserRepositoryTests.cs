using Ardalis.GuardClauses;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Tests.TestExtensions;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Tests.Repositories;

public class FakeUserRepositoryTests
{
    private static readonly FakeUsers FakeUsers = ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers");
    private readonly IUserRepository _sut;
    
    public FakeUserRepositoryTests()
    {
        _sut = new FakeUserRepository();    
    }
    
    [Fact]
    public void Verify_A_User_Can_be_Added()
    {
        // Arrange
        var generatedUserId = Guid.NewGuid();
        
        var user = new User()
        {
            UserId = generatedUserId,
            Email = "peter@test.com",
            HashedPassword = "$2a$14$EeavH1nADA.G/X.XluCm3ef.uxiW5CQCqk0nb/dq0R33s6l57AXxS"
        };
          
        // Act
        var result = _sut.AddUser(user);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Value?.Email);
        Assert.Equal(user.HashedPassword, result.Value?.HashedPassword);
        Assert.Equal(user.UserId, result.Value?.UserId);
    }
    
    [MockDataFact]
    public void Verify_A_User_Can_Be_Retrieved()
    {
        // Arrange
        var loginDetails = FakeUsers.LoginDetailsList?.FirstOrDefault();
        Guard.Against.Null(loginDetails);
        
        // Act
        var result = _sut.GetUser(loginDetails.Email);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Value?.Email, loginDetails.Email);
    }
    
    [Theory]
    [InlineData("test@test.com")]
    [InlineData("larry@last.com")]
    public void Verify_GetUser_With_An_Invalid_Email_Returns_Error(string email)
    {
        // Arrange / Act
        var result = _sut.GetUser(email);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Error, new Error("NOT_FOUND", $"User with {email} was not found"));
    }
    
    [Theory]
    [InlineData("test@test.com")]
    [InlineData("larry@last.com")]
    public void Verify_Invalid_Email_With_Get_User_Returns_Error_(string email)
    {
        // Arrange / Act
        var result = _sut.GetUser(email);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Error, new Error("NOT_FOUND", $"User with {email} was not found"));
    }

    [MockDataFact]
    public void Verify_CheckUserDoesNotExist_With_New_Email_Returns_Success()
    {
        // Arrange
        var userDoesNotExistEmail = "DoesNotExist@test.com";
        
        // Act
        var result = _sut.EnsureUserDoesNotExist(userDoesNotExistEmail);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.IsType<Outcome>(result.Value);
    }
    
    [MockDataFact]
    public void Verify_CheckUserDoesNotExist_With_Existing_Email_Returns_Failed()
    {
        // Arrange
        var userExistsEmail = "demo_user@sample.org";
        
        // Act
        var result = _sut.EnsureUserDoesNotExist(userExistsEmail);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(new Error("ALREADY_REGISTERED", $"User with {userExistsEmail} already registered"), result.Error);
    }
}