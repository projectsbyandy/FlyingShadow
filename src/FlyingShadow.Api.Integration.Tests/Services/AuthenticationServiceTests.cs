using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.TestExtensions;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Services;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Services;
using Guard = Ardalis.GuardClauses.Guard;

namespace FlyingShadow.Api.Integration.Tests.Services;

public class AuthenticationServiceTests
{
    private static readonly FakeUsers FakeUsers = ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers");
    private readonly IAuthenticationService _sut = new AuthenticationService(new FakeUserRepository());
    private const string ValidEmail = "john.doe@sample.org";
    
    # region Validate Credentials
    [MockDataFact]
    public void Verify_Successful_User_Validation()
    {
        // Arrange
        var loginDetails = FakeUsers.LoginDetailsList?.FirstOrDefault(user => user.Email.Equals(ValidEmail));
        Guard.Against.Null(loginDetails);
        
        // Act
        var result = _sut.ValidateCredentials(loginDetails);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(loginDetails.Email, result.Value?.Email);
        Assert.NotEqual(Guid.Empty, result.Value?.UserId);
    }
    
    [MockDataTheory]
    [InlineData("invalid")]
    [InlineData("toast")]
    public void Verify_Unsuccessful_Validation_With_Valid_Email_Invalid_Password(string password)
    {
        // Arrange / Act
        var result = _sut.ValidateCredentials(new LoginDetails()
        {
            Email = ValidEmail,
            Password = password
        });
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, new Error("INVALID_CREDENTIALS", "The email or password provided is incorrect"));
    }
    
    [MockDataTheory]
    [InlineData("test@test.com")]
    [InlineData("peter.doe@sample.org")]
    public void Verify_Unsuccessful_Validation_With_Invalid_Email(string email)
    {
        // Arrange / Act
        var result = _sut.ValidateCredentials(new LoginDetails()
        {
            Email = email,
            Password = "na"
        });
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, new Error("NOT_FOUND", $"User with {email} was not found"));
    }
    #endregion
    
    #region Register User
    
    [MockDataFact]
    public void Verify_A_User_Can_Be_Registered()
    {
        // Arrange
        var user = new RegisterRequest()
        {
            Email = "test@test.com",
            Password = "test123",
        };
        
        // Act
        var result = _sut.Register(user);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value?.UserId);
        Assert.Equal(user.Email, result.Value?.Email);
    }
    
    [MockDataFact]
    public void Verify_An_Existing_User_Cannot_Be_Registered()
    {
        // Arrange
        var existingLoginDetails = FakeUsers.LoginDetailsList?.FirstOrDefault(user => user.Email.Equals(ValidEmail));
        Guard.Against.Null(existingLoginDetails);
        
        // Act
        var result = _sut.Register(existingLoginDetails);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, new Error("ALREADY_REGISTERED", $"User with {existingLoginDetails.Email} already registered"));
    }
    # endregion
}