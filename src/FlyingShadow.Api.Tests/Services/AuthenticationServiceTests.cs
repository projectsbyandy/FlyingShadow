using BCrypt.Net;
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
    private readonly Mock<IPasswordHasher> _passwordHasherFake = new();
    
    private const string UserEmail = "test@test.com";
    private const string UserPassword = "Password123!";
    private const string HashedPassword = "$2a$14$qowgzZE6so8L/GDtNalC4./ODRcaq6za9dV9bur39BbVNtTMGpVBK";
    
    public AuthenticationServiceTests()
    {
        _userRepositoryFake
            .Setup(r => r.GetUser(UserEmail))
            .Returns(Result<User, Error>.Success(new User
            {
                UserId = Guid.Parse("191410b1-9d45-498a-8c2a-b3faf3583fcc"), Email = UserEmail,
                HashedPassword = HashedPassword
            }));

        _userRepositoryFake
            .Setup(r => r.AddUser(It.IsAny<User>()))
            .Returns<User>(Result<User, Error>.Success);

        _userRepositoryFake
            .Setup(r => r.EnsureUserDoesNotExist(UserEmail)).Returns(Result<Outcome, Error>.Success(new Outcome()));
        
        _passwordHasherFake.Setup(hasher => hasher.Hash(It.IsAny<string>())).Returns(HashedPassword);
        _passwordHasherFake.Setup(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        
        _sut = new AuthenticationService(_userRepositoryFake.Object, _passwordHasherFake.Object);
    }
    
    [Fact]
    public void ValidateCredentials_WithValidChecks_ReturnsEmailAndPassword()
    {
        // Arrange
        var loginDetails = new LoginDetails() {Email = UserEmail, Password = UserPassword};
        
        // Act
        var result = _sut.ValidateCredentials(loginDetails);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(loginDetails.Email, result.Value?.Email);
        Assert.NotEqual(Guid.Empty, result.Value?.UserId);
        _passwordHasherFake.Verify(hasher => hasher.Verify(UserPassword, HashedPassword), Times.Once);
    }
    
    [Fact]
    public void ValidateCredentials_UserRepoReturnsUserNotFound_Returns_NotFoundError()
    {
        // Arrange
        _userRepositoryFake
            .Setup(r => r.GetUser(UserEmail))
            .Returns<string>(email => Result<User, Error>.Failure(new Error(ErrorCode.NotFound, $"User:{email} not found")));
        
        // Act
        var result = _sut.ValidateCredentials(new LoginDetails()
        {
            Email = UserEmail,
            Password = "Testing"
        });
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.NotFound, result.Error.Code);
        Assert.Equal($"User:{UserEmail} not found", result.Error.Message);
    }
    
    [Fact]
    public void ValidateCredentials_ValidationReturnFalse_Returns_InvalidCredentialsError()
    {
        // Arrange
        _passwordHasherFake.Setup(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        
        // Act
        var result = _sut.ValidateCredentials(new LoginDetails()
        {
            Email = UserEmail,
            Password = "Testing"
        });
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.InvalidCredentials, result.Error.Code);
        Assert.Equal("The email or password provided is incorrect", result.Error.Message);
    }
    
    [Fact]
    public void ValidateCredentials_HashVerifyThrowsException_Returns_UnexpectedError()
    {
        // Arrange
        _passwordHasherFake.Setup(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>())).Throws(new SaltParseException("Problem with salt value"));

        // Act
        var result = _sut.ValidateCredentials(new LoginDetails()
        {
            Email = UserEmail,
            Password = "Testing"
        });
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.UnexpectedError, result.Error.Code);
        Assert.Equal("Problem with salt value", result.Error.Message);
    }

    [Theory]
    [InlineData("", "Required input user.HashedPassword was empty.")]
    [InlineData(null, "Value cannot be null.")]
    public void ValidateCredentials_SupplyingNullOrEmptyHashedPassword_Returns_Failure(string? badHashedPassword, string errorStartsWith)
    {
        // Arrange
        _userRepositoryFake
            .Setup(r => r.GetUser(UserEmail))
            .Returns(Result<User, Error>.Success(new User
            {
                UserId = Guid.Parse("191410b1-9d45-498a-8c2a-b3faf3583fcc"), Email = UserEmail,
                HashedPassword = badHashedPassword
            }));
        
        // Act
        var result = _sut.ValidateCredentials(new LoginDetails()
        {
            Email = UserEmail,
            Password = "Testing"
        });

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorCode.UnexpectedError, result.Error.Code);
        Assert.StartsWith(errorStartsWith, result.Error.Message);
    }

    [Fact]
    public void Register_With_Valid_Details_Returns_UserDto()
    {
        // Arrange / Act
        var result = _sut.Register(new RegisterRequest()
        {
            Email = UserEmail,
            Password = UserPassword
        });
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(UserEmail, result.Value.Email);
        Assert.NotEqual(Guid.Empty, result.Value.UserId);
        _passwordHasherFake.Verify(hasher => hasher.Hash(UserPassword), Times.Once);
    }
    
    [Fact]
    public void Register_VerifyPasswordThrowsHashException_Returns_Unexpected_Error()
    {
        // Arrange
        _passwordHasherFake.Setup(hasher => hasher.Hash(It.IsAny<string>())).Throws(new SaltParseException("Problem with salt value"));
        
        // Act
        var result = _sut.Register(new RegisterRequest()
        {
            Email = UserEmail,
            Password = UserPassword
        });
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorCode.UnexpectedError, result.Error.Code);
        Assert.Equal("Unable to Register User due to: Problem with salt value", result.Error.Message);
    }
    
    [Fact]
    public void Register_EnsureUserDoesNotExistReturnsFalse_Returns_AlreadyExistsFailure()
    {
        // Arrange
        _userRepositoryFake
            .Setup(r => r.EnsureUserDoesNotExist(UserEmail))
            .Returns<string>(email => Result<Outcome, Error>.Failure(new Error(ErrorCode.AlreadyExists, $"User:{email} already exists")));
        
        // Act
        var result = _sut.Register(new RegisterRequest()
        {
            Email = UserEmail,
            Password = UserPassword
        });
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorCode.AlreadyExists, result.Error.Code);
        Assert.Equal($"User:{UserEmail} already exists", result.Error.Message);
    }
}