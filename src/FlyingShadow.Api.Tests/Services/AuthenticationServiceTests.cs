using FlyingShadow.Api.Services;
using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;
using Moq;

namespace FlyingShadow.Api.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly IAuthenticationService _sut;
    private readonly Mock<IUserRepository> _userRepositoryFake = new();
    
    private const string ValidUserEmail = "test@test.com";
    private const string ValidUserPassword = "Password123!";
    private const string ValidHashedPassword = "$2a$14$qowgzZE6so8L/GDtNalC4./ODRcaq6za9dV9bur39BbVNtTMGpVBK";
    
    public AuthenticationServiceTests()
    {
        _sut = new AuthenticationService(_userRepositoryFake.Object);
    }

    # region Validate Credentials
    
    [Fact]
    public void Verify_Successful_User_Validation()
    {
        // Arrange
        _userRepositoryFake.Setup(r => r.GetUser(ValidUserEmail)).Returns(Result<User, Error>.Success(new User {UserId = Guid.Parse("191410b1-9d45-498a-8c2a-b3faf3583fcc"), Email = ValidUserEmail, HashedPassword = ValidHashedPassword}));

        var loginDetails = new LoginDetails() {Email = ValidUserEmail, Password = ValidUserPassword};
        
        // Act
        var result = _sut.ValidateCredentials(loginDetails);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(loginDetails.Email, result.Value?.Email);
        Assert.NotEqual(Guid.Empty, result.Value?.UserId);
    }
    
    [Theory]
    [InlineData("invalid")]
    [InlineData("toast")]
    public void Verify_Unsuccessful_Validation_With_Valid_Email_Invalid_Password(string password)
    {
        // Arrange
        _userRepositoryFake.Setup(r => r.GetUser(ValidUserEmail)).Returns(Result<User, Error>.Success(new User {UserId = Guid.Parse("191410b1-9d45-498a-8c2a-b3faf3583fcc"), Email = ValidUserEmail, HashedPassword = ValidHashedPassword}));
        
        // Act
        var result = _sut.ValidateCredentials(new LoginDetails()
        {
            Email = ValidUserEmail,
            Password = password
        });
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(new Error(ErrorCode.InvalidCredentials, "The email or password provided is incorrect"), result.Error);
    }
    # endregion
}