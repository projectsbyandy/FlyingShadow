using FlyingShadow.Api.Repositories;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Tests.Repositories;

public class FakeShadowRepositoryTests
{
    [Fact]
    public void Verify_GetAll_Returns_Shadow_Mock_Data()
    {
        // Arrange
        IShadowRepository sut = new FakeShadowRepository();
        
        // Act
        var fakeShadowsResult = sut.GetAll();
        
        // Assert
        Assert.True(fakeShadowsResult.IsSuccess);
        Assert.NotNull(fakeShadowsResult.Value);
        Assert.All(fakeShadowsResult.Value, Assert.NotNull);
    }
}