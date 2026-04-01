using FlyingShadow.Api.Repositories;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Tests.Repositories;

public class FakeStealthMetricsRepositoryTests
{
    [Fact]
    public void Verify_GetAll_Returns_StealthMetrics_Mock_Data()
    {
        // Arrange
        IStealthMetricsRepository sut = new FakeStealthMetricsRepository();
      
        // Act
        var stealthMetricsResult = sut.GetAll();
        
        // Assert
        Assert.True(stealthMetricsResult.IsSuccess);
        Assert.NotNull(stealthMetricsResult.Value);
        Assert.All(stealthMetricsResult.Value, Assert.NotNull);
    }
}