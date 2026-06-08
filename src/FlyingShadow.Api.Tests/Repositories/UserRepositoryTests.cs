using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;
using Moq;

namespace FlyingShadow.Api.Tests.Repositories;

public class UserRepositoryTests : IClassFixture<ShadowDataFixture>
{
    private readonly IUserRepository _sut;
    private readonly ShadowDataFixture _shadowDataFixture;
    private readonly Mock<IQueryProcessor> _queryProcessorMock = new();
    
    public UserRepositoryTests(ShadowDataFixture shadowDataFixture)
    {
        _shadowDataFixture = shadowDataFixture;
        _sut = new UserRepository(_queryProcessorMock.Object);
    }

    [Fact]
    public async Task GetUserAsync_WithUserPresent_ReturnsSuccessWithUser()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp => qp.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<User, Error>.Success(_shadowDataFixture.Users.First()));
        
        // Act
        var result = await _sut.GetUserAsync("tim.h@horton.com");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_shadowDataFixture.Users.First(), result.Value);
    }
    
    [Fact]
    public async Task GetUserAsync_WithNotPresent_ReturnsFailureWithNotFound()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp => qp.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync((string _, string? notFoundMessage, object? _)=> Result<User, Error>.Failure(new Error(ErrorCode.NotFound, notFoundMessage ?? "Default Not Found")));

        const string email = "doesnotexist@test.com";
        
        // Act
        var result = await _sut.GetUserAsync(email);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, $"User with email: {email} not found"), result.Error);
    }
    
    [Fact]
    public async Task AddUserAsync_WithNoErrors_ReturnsSuccess()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp => qp.ExecuteAsync(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<int, Error>.Success(1));

        var userToAdd = _shadowDataFixture.Users.First();

        // Act
        var result = await _sut.AddUserAsync(userToAdd);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userToAdd, result.Value);
    }
    
    [Fact]
    public async Task AddUserAsync_WithConflict_ReturnsFailure()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp => qp.ExecuteAsync(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<int, Error>.Failure(new Error(ErrorCode.Conflict, "User already exists with Id")));

        var userToAdd = _shadowDataFixture.Users.First();
        
        // Act
        var result = await _sut.AddUserAsync(userToAdd);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.Conflict, "User already exists with Id"), result.Error);
    }
    
    [Fact]
    public async Task AddUserAsync_WithUpdatedRows2_ReturnsFailure()
    {
        // Arrange
        const int rowsUpdatedCount = 2;
        _queryProcessorMock
            .Setup(qp => qp.ExecuteAsync(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<int, Error>.Success(rowsUpdatedCount));

        var userToAdd = _shadowDataFixture.Users.First();
        
        // Act
        var result = await _sut.AddUserAsync(userToAdd);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.UnexpectedError, $"Only expecting 1 row updated but got {rowsUpdatedCount}"), result.Error);
    }
    
    [Fact]
    public async Task EnsureUserDoesNotExists_WithUserNotPresent_ReturnsSuccess()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp => qp.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<User, Error>.Failure(new Error(ErrorCode.NotFound, "User already exists with Id")));
        
        // Act
        var result = await _sut.EnsureUserDoesNotExistAsync("test@newuser.com");
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(new Outcome(), result.Value);
    }
    
    [Fact]
    public async Task EnsureUserDoesNotExists_WithUserExists_ReturnsAlreadyRegisteredFailure()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp => qp.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync((string _, string? notFoundMessage, object? _)=> Result<User, Error>.Success(_shadowDataFixture.Users.First()));

        const string existingEmail = "test@test.com";
        
        // Act
        var result = await _sut.EnsureUserDoesNotExistAsync(existingEmail);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.AlreadyExists, $"User with {existingEmail} already registered"), result.Error);
    }
}