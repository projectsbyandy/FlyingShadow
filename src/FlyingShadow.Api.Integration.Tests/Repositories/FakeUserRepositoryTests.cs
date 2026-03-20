using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Repositories.Internal;
using FlyingShadow.Api.Utils;
using Ardalis.GuardClauses;
using FlyingShadow.Api.DTO.ResultType;
using FlyingShadow.Api.Integration.Tests.TestExtensions;
using Assert = Xunit.Assert;

namespace FlyingShadow.Api.Integration.Tests.Repositories;

public class FakeUserRepositoryTests
{
    private static readonly Configuration Config = ConfigReader.GetConfiguration<Configuration>();
    private readonly IUserRepository _sut;
    
    public FakeUserRepositoryTests()
    {
        _sut = new FakeUserRepository(Config);    
    }
    
    [MockDataFact]
    public void Verify_A_User_Can_be_Added()
    {
        // Arrange
        var user = new User()
        {
            Email = "andy@test.com",
            Password = "password2"
        };
          
        // Act
        var result = _sut.AddUser(user);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Value);
    }
    
    [MockDataFact]
    public void Verify_A_User_Can_Be_Retrieved()
    {
        // Arrange
        var user = Config.FakeUsers?.Users?.FirstOrDefault();
        Guard.Against.Null(user);
        
        // Act
        var result = _sut.GetUser(user.Email);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Value?.Email, user.Email);
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
}