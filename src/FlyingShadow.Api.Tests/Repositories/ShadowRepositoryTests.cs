using FlyingShadow.Api.Repositories;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using Moq;

namespace FlyingShadow.Api.Tests.Repositories;

public class ShadowRepositoryTests
{
    private readonly IShadowRepository _sut;
    private readonly Mock<IQueryProcessor> _queryProcessorMock = new();
    
    public ShadowRepositoryTests()
    {
        _sut = new ShadowRepository(_queryProcessorMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenQuerySucceeds_ReturnAllShadows()
    {
        // Arrange
        _queryProcessorMock.Setup(qp => qp.QueryAsync<Shadow>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<IEnumerable<Shadow>, Error>.Success(new List<Shadow>()));

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
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
    public async Task GetByCodeNameAsync_WhenQuerySucceeds_ReturnsShadow()
    {
        // Arrange
        var shadow = new Shadow()
        {
            CodeName = "code",
            Clan = "clan",
            Id = Guid.NewGuid(),
            Origin = "origin",
            Rank = Rank.Danza
        };

        _queryProcessorMock.Setup(qp => qp.QuerySingleOrDefaultAsync<Shadow>(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<object?>()))
        .ReturnsAsync(Result<Shadow, Error>.Success(shadow));
        
        // Act
        var result = await _sut.GetByCodeNameAsync(shadow.CodeName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(shadow, result.Value);
    }
}