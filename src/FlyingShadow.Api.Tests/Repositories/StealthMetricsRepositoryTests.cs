using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using Moq;

namespace FlyingShadow.Api.Tests.Repositories;

public class StealthMetricsRepositoryTests : IClassFixture<ShadowDataFixture>
{
    private readonly IStealthMetricsRepository _sut;
    private readonly ShadowDataFixture _shadowDataFixture;
    private readonly Mock<IQueryProcessor> _queryProcessorMock = new();
    
    public StealthMetricsRepositoryTests(ShadowDataFixture shadowDataFixture)
    {
        _shadowDataFixture = shadowDataFixture;
        _sut = new StealthMetricsRepository(_queryProcessorMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessfulResultWithStealthMetrics()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp =>
                qp.QueryAsync<StealthMetrics>(It.IsAny<string>()))
            .ReturnsAsync(Result<IEnumerable<StealthMetrics>, Error>.Success(_shadowDataFixture.StealthMetrics));
            
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_shadowDataFixture.StealthMetrics, result.Value);
    }

    [Fact]
    public async Task GetByShadowIdAsync_WithData_ReturnsSuccessfulResultWithStealthMetrics()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp =>
                qp.QuerySingleOrDefaultAsync<StealthMetrics>(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<object?>()))
            .ReturnsAsync(Result<StealthMetrics, Error>.Success(_shadowDataFixture.StealthMetrics.First()));
        
        // Act
        var result = await _sut.GetByShadowIdAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_shadowDataFixture.StealthMetrics.First(), result.Value);
    }

    [Fact]
    public async Task GetByShadowIdAsync_WithMissingData_ReturnsFailureWithNotFoundMessage()
    {
        // Arrange
        _queryProcessorMock
            .Setup(qp => qp.QuerySingleOrDefaultAsync<StealthMetrics>(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<object?>()))
            .ReturnsAsync((string _, string? notFoundMessage, object?_) => Result<StealthMetrics, Error>.Failure(new Error(ErrorCode.NotFound, notFoundMessage ?? "Shadow not found")));
        
        // Act
        var result = await _sut.GetByShadowIdAsync(Guid.Parse("5b6dde15-8099-498a-b02a-cfe3c7278113"));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, "Stealth Metrics with ShadowId: 5b6dde15-8099-498a-b02a-cfe3c7278113 not found"), result.Error);
    }
}