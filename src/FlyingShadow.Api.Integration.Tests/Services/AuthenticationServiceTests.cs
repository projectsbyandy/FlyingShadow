using Ardalis.GuardClauses;
using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.DTO.ResultType;
using FlyingShadow.Api.Integration.Tests.TestExtensions;
using FlyingShadow.Api.Repositories.Internal;
using FlyingShadow.Api.Services;
using FlyingShadow.Api.Services.Internal;
using FlyingShadow.Api.Utils;
using Guard = Ardalis.GuardClauses.Guard;
using Assert = Xunit.Assert;

namespace FlyingShadow.Api.Integration.Tests.Services;

public class AuthenticationServiceTests
{
    private static readonly Configuration Config = ConfigReader.GetConfiguration<Configuration>();
    private readonly IAuthenticationService _sut = new AuthenticationService(new FakeUserRepository(Config));
    private const string ValidEmail = "john.doe@sample.org";
    
    [MockDataFact]
    public void Verify_Successful_User_Validation()
    {
        // Arrange
        var user = Config.FakeUsers?.Users?.FirstOrDefault(user => user.Email.Equals(ValidEmail));
        Guard.Against.Null(user);
        
        // Act
        var result = _sut.ValidateCredentials(user);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, user);
    }
    
    [MockDataTheory]
    [InlineData("invalid")]
    [InlineData("toast")]
    public void Verify_Unsuccessful_Validation_With_Valid_Email_Invalid_Password(string password)
    {
        // Arrange / Act
        var result = _sut.ValidateCredentials(new User()
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
        var result = _sut.ValidateCredentials(new User()
        {
            Email = email,
            Password = "na"
        });
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, new Error("NOT_FOUND", $"User with {email} was not found"));
    }
}