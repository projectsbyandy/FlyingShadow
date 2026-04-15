using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.Models.Battle;

namespace FlyingShadow.Core.Tests.Models.Battle;

public class BattleReportTests
{
    private readonly Stats _shadowOneStats = new()
    {
        CodeName = "Shadow Warrior",
        CombatPower = 11.11,
        EvasionIndex = 22.22,
        StealthScore = 33.33,
        OverallRating = 44.44
    };

    private readonly Stats _shadowTwoStats = new()
    {
        CodeName = "Shadow Prince",
        CombatPower = 55.55,
        EvasionIndex = 66.66,
        StealthScore = 77.77,
        OverallRating = 88.88
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