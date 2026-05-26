using FlyingShadow.Api.Repositories;
using FlyingShadow.Core.Repositories;
using Ardalis.GuardClauses;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Tests.Repositories;

public class FakeShadowRepositoryTests
{
    private readonly IShadowRepository _sut;

    public FakeShadowRepositoryTests()
    {
        _sut = new FakeShadowRepository();
    }

    [Fact]
    public async Task GetAll_ReturnsShadowMockData()
    {
        // Arrange / Act
        var fakeShadowsResult = await _sut.GetAllAsync();
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.All(fakeShadowsResult.Value, Assert.NotNull);
    }
    
    [Fact]
    public async Task GetByCodeName_WithValidCodeName_ReturnsShadowMockData()
    {
        // Arrange
        var shadows = await _sut.GetAllAsync();
        var firstShadow = Guard.Against.Null(shadows.Value.First());    

        // Act
        var fakeShadowsResult = await _sut.GetByCodeNameAsync(firstShadow.CodeName);
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.Equal(firstShadow, fakeShadowsResult.Value);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByCodeName_WithValidCodeNameWithMixedCasing_ReturnsShadowMockData(bool isUpper)
    {
        // Arrange
        var shadows = await _sut.GetAllAsync();
        var firstShadow = Guard.Against.Null(shadows.Value.First());    
        var codeNameWithMixedCasing = isUpper ? firstShadow.CodeName.ToUpper() : firstShadow.CodeName.ToLower();
        
        // Act
        var fakeShadowsResult = await _sut.GetByCodeNameAsync(codeNameWithMixedCasing);
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.Equal(firstShadow, fakeShadowsResult.Value);
    }
    
    [Fact]
    public async Task GetShadowByCodeName_WithInValidCodeName_ReturnsNotFoundErrorCode()
    {
        // Arrange
        const string doesNotExistCodeName = "Shadow Dilbert";   

        // Act
        var fakeShadowsResult = await _sut.GetByCodeNameAsync(doesNotExistCodeName);
        
        // Assert
        Assert.True(fakeShadowsResult.IsFailure);
        Assert.NotNull(fakeShadowsResult.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, $"Shadow code name: {doesNotExistCodeName} does not exist"), fakeShadowsResult.Error);
    }
}