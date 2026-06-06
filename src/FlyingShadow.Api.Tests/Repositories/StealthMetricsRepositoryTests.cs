using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using Moq;

namespace FlyingShadow.Api.Tests.Repositories;

public class StealthMetricsRepositoryTests : ShadowDataFixture
{
    private readonly IStealthMetricsRepository _sut;
    private readonly Mock<IQueryProcessor> _queryProcessorMock = new();
    
    public StealthMetricsRepositoryTests()
    {
        _sut = new StealthMetricsRepository(_queryProcessorMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessfulResultWithStealthMetrics()
    {
        // Arrange
        _queryProcessorMock.Setup(qp =>
                qp.QueryAsync<StealthMetrics>(It.IsAny<string>()))
            .ReturnsAsync(Result<IEnumerable<StealthMetrics>, Error>.Success(StealthMetrics));
            
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(StealthMetrics, result.Value);
    }
}