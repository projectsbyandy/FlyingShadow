using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.Models.Battle;

namespace FlyingShadow.Core.Tests.Models.Battle;

public class BattleReportTests
{
    private readonly Stats _shadowOneStats = new()
    {
        CodeName = "Shadow Warrior",
        CombatPower = 11.11m,
        EvasionIndex = 22.22m,
        StealthScore = 33.33m,
        OverallRating = 44.44m
    };

    private readonly Stats _shadowTwoStats = new()
    {
        CodeName = "Shadow Prince",
        CombatPower = 55.55m,
        EvasionIndex = 66.66m,
        StealthScore = 77.77m,
        OverallRating = 88.88m
    };

    private readonly StatResults _statBreakdown = new()
    {
        CombatPowerWinner = "Shadow Prince",
        EvasionIndexWinner = "Shadow Prince",
        StealthScoreWinner = "Shadow Prince",
    };

    private const string Outcome = "Shadow Prince";

    [Theory]
    [InlineData("Shadow Warrior")]
    [InlineData("Shadow Beast")]
    public void Verify_BattleReport_With_Win_Converts_To_BattleResponse(string outcome)
    {
        // Arrange
        var battleReport = new BattleReport {
            Outcome = outcome,
            ShadowOneStats = _shadowOneStats,
            ShadowTwoStats = _shadowTwoStats,
            StatBreakdown = _statBreakdown
        };

        var expectedBattleResponse = new BattleResponse()
        {
            Outcome = $"{outcome} Wins!",
            ShadowOneStats = _shadowOneStats,
            ShadowTwoStats = _shadowTwoStats,
            StatBreakdown = _statBreakdown
        };
        
        // Act
        var battleResponse = battleReport.ToBattleResponse();

        // Assert
        Assert.Equal(expectedBattleResponse, battleResponse);
    }
    
    [Fact]
    public void Verify_BattleReport_DrawOutcome_Converts_To_BattleResponse()
    {
        // Arrange
        var battleReport = new BattleReport {
            Outcome = "Draw",
            ShadowOneStats = _shadowOneStats,
            ShadowTwoStats = _shadowTwoStats,
            StatBreakdown = _statBreakdown
        };
        
        // Act
        var battleResponse = battleReport.ToBattleResponse();

        // Assert
        Assert.Equal("Draw!", battleResponse.Outcome);
    }
}