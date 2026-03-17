using Ardalis.GuardClauses;
using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.Repositories.Internal;
using FlyingShadow.Api.Services;
using FlyingShadow.Api.Services.Internal;
using FlyingShadow.Api.Utils;
using Xunit;
using Guard = Ardalis.GuardClauses.Guard;
using Assert = Xunit.Assert;

namespace FlyingShadow.Api.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly IAuthenticationService _sut = new AuthenticationService(new FakeUserRepository());
    private const string ValidEmail = "john.doe@sample.org";

    [Fact]
    public void Verify_Successful_User_Validation()
    {
        // Arrange
        var config = ConfigReader.GetConfiguration<Configuration>();
        var user = config.ValidFakeUsers?.FirstOrDefault(user => user.Email.Equals(ValidEmail));
        Guard.Against.Null(user);
        
        // Act / Assert
        Assert.True(_sut.ValidateCredentials(user));
    }
    
    [Theory]
    [InlineData("invalid")]
    [InlineData("toast")]
    public void Verify_Unsuccessful_Validation_With_Valid_Email_Invalid_Password(string password)
    {
        Assert.False(_sut.ValidateCredentials(new User()
        {
            Email = ValidEmail,
            Password = password
        }));
    }
}