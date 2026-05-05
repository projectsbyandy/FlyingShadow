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
    public void GetAll_ReturnsShadowMockData()
    {
        // Arrange / Act
        var fakeShadowsResult = _sut.GetAll();
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.All(fakeShadowsResult.Value, Assert.NotNull);
    }
    
    [Fact]
    public void GetByCodeName_WithValidCodeName_ReturnsShadowMockData()
    {
        // Arrange
        var firstShadow = Guard.Against.Null(_sut.GetAll().Value.First());    

        // Act
        var fakeShadowsResult = _sut.GetByCodeName(firstShadow.CodeName);
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.Equal(firstShadow, fakeShadowsResult.Value);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetByCodeName_WithValidCodeNameWithMixedCasing_ReturnsShadowMockData(bool isUpper)
    {
        // Arrange
        var firstShadow = Guard.Against.Null(_sut.GetAll().Value.First());    
        var codeNameWithMixedCasing = isUpper ? firstShadow.CodeName.ToUpper() : firstShadow.CodeName.ToLower();
        
        // Act
        var fakeShadowsResult = _sut.GetByCodeName(codeNameWithMixedCasing);
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.Equal(firstShadow, fakeShadowsResult.Value);
    }
    
    [Fact]
    public void GetShadowByCodeName_WithInValidCodeName_ReturnsNotFoundErrorCode()
    {
        // Arrange
        const string doesNotExistCodeName = "Shadow Dilbert";   

        // Act
        var fakeShadowsResult = _sut.GetByCodeName(doesNotExistCodeName);
        
        // Assert
        Assert.True(fakeShadowsResult.IsFailure);
        Assert.NotNull(fakeShadowsResult.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, $"Shadow code name: {doesNotExistCodeName} does not exist"), fakeShadowsResult.Error);
    }
}