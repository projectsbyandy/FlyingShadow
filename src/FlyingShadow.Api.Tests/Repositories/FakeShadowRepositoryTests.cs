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
    public void Verify_GetAll_Returns_Shadow_Mock_Data()
    {
        // Arrange / Act
        var fakeShadowsResult = _sut.GetAll();
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.All(fakeShadowsResult.Value, Assert.NotNull);
    }
    
    [Fact]
    public void Verify_GetShadowByCodeName_With_Valid_CodeName_Returns_Shadow_Mock_Data()
    {
        // Arrange
        var firstShadow = Guard.Against.Null(_sut.GetAll().Value?.First());    

        // Act
        var fakeShadowsResult = _sut.GetByCodeName(firstShadow.CodeName);
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.Equal(firstShadow, fakeShadowsResult.Value);
    }
    
    [Fact]
    public void Verify_GetShadowByCodeName_With_InValid_CodeName_Returns_Not_Found_ErrorCode()
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