using FlyingShadow.Api.Services;
using FlyingShadow.Core.DTO.Authenticate;
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
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, new Error("INVALID_CREDENTIALS", "The email or password provided is incorrect"));
    }
    
    [Theory]
    [InlineData("invalid")]
    [InlineData("toast")]
    public void Verify_Unsuccessful_Password_Validation_Using_Hashed_Password_With_Invalid_Salt(string invalidHashedPassword)
    {
        // Arrange
        _userRepositoryFake.Setup(r => r.GetUser(ValidUserEmail)).Returns(Result<User, Error>.Success(new User {UserId = Guid.Parse("191410b1-9d45-498a-8c2a-b3faf3583fcc"), Email = ValidUserEmail, HashedPassword = invalidHashedPassword}));
        
        // Act
        var result = _sut.ValidateCredentials(new LoginDetails()
        {
            Email = ValidUserEmail,
            Password = ValidUserPassword
        });
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, new Error("UNABLE_TO_VALIDATE", "Invalid salt version"));
    }
    
    [Theory]
    [InlineData("tester@test.com")]
    [InlineData("peter.doe@sample.org")]
    public void Verify_Unsuccessful_Validation_With_Invalid_Email(string email)
    {
        // Arrange
        _userRepositoryFake.Setup(r => r.GetUser(It.IsAny<string>())).Returns(Result<User, Error>.Failure(new Error("NOT_FOUND", $"User with {email} was not found")));

        // Act
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
    
    [Fact]
    public void Verify_A_User_Can_Be_Registered()
    {
        // Arrange
        var user = new RegisterRequest()
        {
            Email = "test@test.com",
            Password = "test123",
        };
        
        _userRepositoryFake.Setup(r => r.EnsureUserDoesNotExist(user.Email))
            .Returns(Result<Outcome, Error>.Success(new()));
            
        _userRepositoryFake.Setup(r => r.AddUser(It.IsAny<User>())).Returns(Result<User, Error>.Success(new()
        {
            Email = user.Email,
            HashedPassword = It.IsAny<string>()
        }));
        
        // Act
        var result = _sut.Register(user);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value?.UserId);
        Assert.Equal(user.Email, result.Value?.Email);
    }
    
    [Fact]
    public void Verify_An_Existing_User_Cannot_Be_Registered()
    {
        // Arrange
        var existingLoginDetails = new RegisterRequest()
        {
            Email = "already_exist@test.com",
            Password = "test123"
        };
        
        _userRepositoryFake.Setup(r => r.EnsureUserDoesNotExist(existingLoginDetails.Email))
            .Returns(Result<Outcome, Error>.Failure(new Error("ALREADY_REGISTERED", $"User with {existingLoginDetails.Email} already registered")));
        
        // Act
        var result = _sut.Register(existingLoginDetails);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, new Error("ALREADY_REGISTERED", $"User with {existingLoginDetails.Email} already registered"));
    }
    # endregion
}