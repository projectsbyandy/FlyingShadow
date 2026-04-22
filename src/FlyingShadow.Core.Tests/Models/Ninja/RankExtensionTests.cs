using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Tests.Models.Ninja;

public class RankExtensionTests
{
    [Theory]
    [InlineData(Rank.Danza, 1)]
    [InlineData(Rank.Toshiyama, 2)]
    [InlineData(Rank.Oniwaban, 3)]
    public void ToMultiply_WithValidRank_ReturnsValidMultiplier(Rank rank,
        int expectedMultiplier)
    {
        // Arrange / Act
        var factor = rank.ToMultiplier();

        // Assert
        Assert.Equal(expectedMultiplier, factor);
    }

    [Fact]
    public void ToMultiplier_WithInvalidRank_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidLevel = (Rank)99;

        // Act
        Action act = () => invalidLevel.ToMultiplier();

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Equal("rank", exception.ParamName);
        Assert.StartsWith("Unknown rank", exception.Message);
    }
}