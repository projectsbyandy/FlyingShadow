using FlyingShadow.Api.Repositories;
using FlyingShadow.Core.Repositories;
using Ardalis.GuardClauses;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Tests.Repositories;

public class FakeStealthMetricsRepositoryTests
{
    private readonly FakeStealthMetricsRepository _sut;

    public FakeStealthMetricsRepositoryTests()
    {
        _sut = new FakeStealthMetricsRepository();
    }

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

    [Fact] public void Verify_GetMetricsByShadowId_With_Valid_Id_Returns_StealthMetrics_Mock_Data()
    {
        // Arrange
        var firstStealthMetrics = Guard.Against.Null(_sut.GetAll().Value?.First());    

        // Act
        var fakeStealthMetricsResult = _sut.GetByShadowId(firstStealthMetrics.ShadowId);
        
        // Assert
        Assert.True(fakeStealthMetricsResult.IsSuccess);
        Assert.NotNull(fakeStealthMetricsResult.Value);
        Assert.Equal(firstStealthMetrics, fakeStealthMetricsResult.Value);
    }
    
    [Fact]
    public void Verify_GetMetricsByShadowId_With_Valid_Id_Returns_Not_Found_Error_Code()
    {
        // Arrange
        var doesNotExistShadowId = Guid.NewGuid();

        // Act
        var fakeStealthMetricsResult = _sut.GetByShadowId(doesNotExistShadowId);
        
        // Assert
        Assert.True(fakeStealthMetricsResult.IsFailure);
        Assert.NotNull(fakeStealthMetricsResult.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, $"Stealth metrics with ShadowId: {doesNotExistShadowId} does not exist"), fakeStealthMetricsResult.Error);
    }
}