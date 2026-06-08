using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using Moq;

namespace FlyingShadow.Api.Tests.Repositories;

public class ShadowRepositoryTests : IClassFixture<ShadowDataFixture>
{
    private readonly IShadowRepository _sut;
    private readonly ShadowDataFixture _shadowDataFixture;
    private readonly Mock<IQueryProcessor> _queryProcessorMock = new();
    
    public ShadowRepositoryTests(ShadowDataFixture shadowDataFixture)
    {
        _shadowDataFixture = shadowDataFixture;
        _sut = new ShadowRepository(_queryProcessorMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenQuerySucceeds_ReturnAllShadows()
    {
        // Arrange
        _queryProcessorMock.Setup(qp => qp.QueryAsync<Shadow>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<IEnumerable<Shadow>, Error>.Success(_shadowDataFixture.Shadows));

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_shadowDataFixture.Shadows, result.Value);
    }
    
    [Fact]
    public async Task GetAllAsync_WhenQueryFails_ReturnsFailure()
    {
        // Arrange
        var error = new Error(ErrorCode.DbConnectionProblem, "Connection failed");
        
        _queryProcessorMock
            .Setup(qp => qp.QueryAsync<Shadow>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<IEnumerable<Shadow>, Error>.Failure(error));

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }
    
    [Fact]
    public async Task GetByCodeNameAsync_WhenQuerySucceeds_ReturnsSuccessWithShadow()
    {
        // Arrange
        var shadow = _shadowDataFixture.Shadows.First();
        
        _queryProcessorMock.Setup(qp => qp.QuerySingleOrDefaultAsync<Shadow>(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<object?>()))
        .ReturnsAsync(Result<Shadow, Error>.Success(shadow));
        
        // Act
        var result = await _sut.GetByCodeNameAsync(shadow.CodeName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(shadow, result.Value);
    }
    
    [Fact]
    public async Task GetByCodeNameAsync_WhenNotCodeNameExists_ReturnsFailureWithCorrectMessage()
    {
        // Arrange
        _queryProcessorMock.Setup(qp => qp.QuerySingleOrDefaultAsync<Shadow>(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<object?>()))
            .ReturnsAsync((string _, string? notFoundMessage, object? _) => Result<Shadow, Error>.Failure(new Error(ErrorCode.NotFound, notFoundMessage ?? "default item not found")));
        
        // Act
        var result = await _sut.GetByCodeNameAsync("DoesNotExist");

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, "Shadow with CodeName: DoesNotExist not found"), result.Error);
    }
}