using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Tests.Models.Ninja;

public class AcrobaticLevelExtensionTests
{
    [Theory]
    [InlineData(AcrobaticsLevel.Beginner, 1)]
    [InlineData(AcrobaticsLevel.Intermediate, 2)]
    [InlineData(AcrobaticsLevel.Advanced, 3)]
    public void ToMultiply_WithValidAcrobaticsLevel_ReturnsValidMultiplier(AcrobaticsLevel acrobaticsLevel,
        int expectedMultiplier)
    {
        // Arrange / Act
        var factor = acrobaticsLevel.ToMultiplier();

        // Assert
        Assert.Equal(expectedMultiplier, factor);
    }

    [Fact]
    public void ToMultiplier_WithInvalidAcrobaticsLevel_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidLevel = (AcrobaticsLevel)99;

        // Act
        Action act = () => invalidLevel.ToMultiplier();

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Equal("level", exception.ParamName);
        Assert.StartsWith("Unknown acrobatics level", exception.Message);
    }
}