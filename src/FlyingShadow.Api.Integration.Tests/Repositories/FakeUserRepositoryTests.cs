using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Repositories.Internal;
using FlyingShadow.Api.Utils;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.TestExtensions;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Models.Users;

namespace FlyingShadow.Api.Integration.Tests.Repositories;

public class FakeUserRepositoryTests
{
    private static readonly FakeUsers FakeUsers = ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers");
    private readonly IUserRepository _sut;
    
    public FakeUserRepositoryTests()
    {
        _sut = new FakeUserRepository();    
    }
    
    [MockDataFact]
    public void Verify_A_User_Can_be_Added()
    {
        // Arrange
        var generatedUserId = Guid.NewGuid();
        
        var user = new User()
        {
            UserId = generatedUserId,
            Email = "andy@test.com",
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
    
    [MockDataTheory]
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
    
    [MockDataTheory]
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
}