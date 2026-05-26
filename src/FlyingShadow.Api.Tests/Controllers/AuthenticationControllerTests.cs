using FlyingShadow.Api.Controllers;
using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.DTO.Token;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlyingShadow.Api.Tests.Controllers;

public class AuthenticationControllerTests
{
    private readonly AuthenticationController _sut;
    private readonly Mock<IAuthenticationService> _authenticationServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;

    private readonly LoginDetails _loginDetails = new()
    {
        Email = "Larry@test.com",
        Password = "password123"
    };
    
    private readonly RegisterRequest _registerRequest = new()
    {
        Email = "Larry@test.com",
        Password = "password123"
    };
    
    public AuthenticationControllerTests()
    {
        _authenticationServiceMock = new Mock<IAuthenticationService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _sut = new AuthenticationController(_authenticationServiceMock.Object, _tokenServiceMock.Object);
    }
    
    #region Login
    
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithTokenAndExpiry()
    {
        // Arrange
        _authenticationServiceMock.Setup(auth => auth.ValidateCredentialsAsync(It.IsAny<LoginDetails>()))
            .ReturnsAsync((LoginDetails loginDetails) => Result<UserDto, Error>.Success(new UserDto()
            {
                UserId = Guid.NewGuid(),
                Email = loginDetails.Email
            }));

        const string generatedJwt = "ThisIsTheGeneratedToken";
        var generatedDateTimeExpiry = DateTime.UtcNow.AddHours(1);
        
        _tokenServiceMock.Setup(service => service.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Result<TokenDetails, Error>.Success(new TokenDetails(generatedJwt, generatedDateTimeExpiry)));
        
        // Act
        var actionResult = await _sut.LoginAsync(_loginDetails);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal(generatedJwt, loginResponse.TokenDetails?.Token);
        Assert.Equal(generatedDateTimeExpiry, loginResponse.TokenDetails?.ExpiresAt);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        _authenticationServiceMock.Setup(auth => auth.ValidateCredentialsAsync(It.IsAny<LoginDetails>()))
            .ReturnsAsync(Result<UserDto, Error>.Failure(new Error(ErrorCode.InvalidCredentials, "The email or password provided is incorrect")));
        
        // Act
        var actionResult = await _sut.LoginAsync(_loginDetails);
        
        // Assert
        var unauthorizedObjectResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        var errorResponse = Assert.IsType<ErrorResponse>(unauthorizedObjectResult.Value);
        Assert.Equal("Invalid Email or Password", errorResponse.Message);
    }
    
    #endregion
    
    #region Register

    [Fact]
    public async Task Register_WithValidDetails_ReturnsCreatedWithUserId()
    {
        // Arrange
        var generatedUserId = Guid.NewGuid();
        _authenticationServiceMock.Setup(auth => auth.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync((RegisterRequest registerRequest) => Result<UserDto, Error>.Success(new UserDto()
            {
                UserId = generatedUserId,
                Email = registerRequest.Email,
            }));
        
        // Act
        var actionResult = await _sut.RegisterAsync(_registerRequest);
        
        // Assert
        var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
        var registerResponse = Assert.IsType<RegisterResponse>(createdResult.Value);
        Assert.Equal(generatedUserId, registerResponse.UserId);
    }
    
    [Fact]
    public async Task Register_WithAnExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        _authenticationServiceMock.Setup(auth => auth.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync((RegisterRequest registerRequest) => Result<UserDto, Error>.Failure(new Error(ErrorCode.AlreadyExists, $"User with {registerRequest.Email} already registered")));
        
        // Act
        var actionResult = await _sut.RegisterAsync(_registerRequest);
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Registration could not be completed.", errorResponse.Message);
    }
    
    [Fact]
    public async Task Register_WithServiceError_Returns500()
    {
        // Arrange
        _authenticationServiceMock.Setup(auth => auth.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync((RegisterRequest registerRequest) => Result<UserDto, Error>.Failure(new Error(ErrorCode.UnexpectedError, $"Problem writing {registerRequest.Email} to DB")));
        
        // Act
        var actionResult = await _sut.RegisterAsync(_registerRequest);
        
        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal("An unexpected error occurred.", errorResponse.Message);
    }
    
    #endregion
}