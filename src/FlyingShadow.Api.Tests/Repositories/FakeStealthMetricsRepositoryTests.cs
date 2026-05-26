using FlyingShadow.Api.Repositories;
using FlyingShadow.Core.Repositories;
using Ardalis.GuardClauses;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Tests.Repositories;

public class FakeStealthMetricsRepositoryTests
{
    private readonly IStealthMetricsRepository _sut;

    public FakeStealthMetricsRepositoryTests()
    {
        _sut = new FakeStealthMetricsRepository();
    }

    [Fact]
    public async Task GetAll_ReturnsStealthMetricsMockData()
    {
        // Arrange
        IStealthMetricsRepository sut = new FakeStealthMetricsRepository();
      
        // Act
        var stealthMetricsResult = await sut.GetAllAsync();
        
        // Assert
        Assert.True(stealthMetricsResult.IsSuccess);
        Assert.NotNull(stealthMetricsResult.Value);
        Assert.All(stealthMetricsResult.Value, Assert.NotNull);
    }

    [Fact] 
    public async Task GetByShadowId_WithValidId_ReturnsStealthMetricsMockData()
    {
        // Arrange
        var metricsResult = await _sut.GetAllAsync();
        var firstStealthMetrics = Guard.Against.Null(metricsResult.Value.First());    

        // Act
        var fakeStealthMetricsResult = await _sut.GetByShadowIdAsync(firstStealthMetrics.ShadowId);
        
        // Assert
        Assert.True(fakeStealthMetricsResult.IsSuccess);
        Assert.NotNull(fakeStealthMetricsResult.Value);
        Assert.Equal(firstStealthMetrics, fakeStealthMetricsResult.Value);
    }
    
    [Fact]
    public async Task GetByShadowId_WithValidId_ReturnsNotFoundErrorCode()
    {
        // Arrange
        var doesNotExistShadowId = Guid.NewGuid();

        // Act
        var fakeStealthMetricsResult = await _sut.GetByShadowIdAsync(doesNotExistShadowId);
        
        // Assert
        Assert.True(fakeStealthMetricsResult.IsFailure);
        Assert.NotNull(fakeStealthMetricsResult.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, $"Stealth metrics with ShadowId: {doesNotExistShadowId} does not exist"), fakeStealthMetricsResult.Error);
    }
}