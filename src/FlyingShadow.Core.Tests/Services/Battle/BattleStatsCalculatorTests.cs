using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Battle;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Services.Battle;
using FlyingShadow.Core.Tests.Fixtures;

namespace FlyingShadow.Core.Tests.Services.Battle;

public class BattleStatsCalculatorTests : ShadowDtoFixture
{
    [Theory]
    [MemberData(nameof(RankTestData))]
    public void Process_ShadowDto_With_Different_Ranks_Returns_Correct_Stats(Rank rank, Stats expectedStats)
    {
        // Arrange
        var shadowUpdatedRank = ShadowOne with
        {
            Rank = rank
        };
        
        // Act
        var stats = BattleStatsCalculator.Process(shadowUpdatedRank);
        
        // Assert
        Assert.Equal(expectedStats with { CodeName = ShadowOne.CodeName }, stats);
    }
    
    public static IEnumerable<object[]> RankTestData =>
    [
        [Rank.Danza, new Stats {CodeName = "", OverallRating = 1420.26m, CombatPower = 3432m, EvasionIndex  = 27.393m, StealthScore = 151.5m}],
        [Rank.Oniwaban, new Stats {CodeName = "", OverallRating = 4165.86m, CombatPower = 10296m, EvasionIndex  = 27.393m, StealthScore = 151.5m}],
        [Rank.Toshiyama, new Stats {CodeName = "", OverallRating = 2793.06m, CombatPower = 6864m, EvasionIndex  = 27.393m, StealthScore = 151.5m}],
    ];

    [Theory]
    [MemberData(nameof(AcrobaticTestData))]
    public void Process_ShadowDto_With_Different_Acrobatics_Returns_Correct_Stats(AcrobaticsLevel acrobaticsLevel, Stats expectedStats)
    {
        // Arrange
        var shadowUpdatedAcrobaticsLevel = ShadowOne with
        {
            ShadowSkills = ShadowOne.ShadowSkills with
            {
                AcrobaticsLevel = acrobaticsLevel
            }
        };
        
        // Act
        var stats = BattleStatsCalculator.Process(shadowUpdatedAcrobaticsLevel);
        
        // Assert
        Assert.Equal(expectedStats with { CodeName = ShadowOne.CodeName }, stats);
    }
    
    public static IEnumerable<object[]> AcrobaticTestData =>
    [
        [AcrobaticsLevel.Beginner, new Stats {CodeName = "", OverallRating = 1388.62m, CombatPower = 3432m, EvasionIndex  = 9.131m, StealthScore = 50.5m}],
        [AcrobaticsLevel.Intermediate, new Stats {CodeName = "", OverallRating = 2777.24m, CombatPower = 6864m, EvasionIndex  = 18.262m, StealthScore = 101m}],
        [AcrobaticsLevel.Advanced, new Stats {CodeName = "", OverallRating = 4165.86m, CombatPower = 10296m, EvasionIndex  = 27.393m, StealthScore = 151.5m}],
    ];

    [Theory]
    [MemberData(nameof(ShadowStatsTestData))]
    public void Process_ShadowDto_With_Different_Shadow_Skills_Returns_Correct_Stats(int invisbilityDuration, int shadowBlendScore, int silenceRating, Stats expectedStats)
    {
        // Arrange
        var shadowUpdatedSkills = ShadowOne with
        {
            ShadowSkills = ShadowOne.ShadowSkills with
            {
                InvisibilityDurationMs = invisbilityDuration,
                ShadowBlendScore = shadowBlendScore,
                SilenceRating = silenceRating
            }
        };
        
        // Act
        var stats = BattleStatsCalculator.Process(shadowUpdatedSkills);

        // Assert
        Assert.Equal(expectedStats with { CodeName = ShadowOne.CodeName }, stats);
    }
    
    public static IEnumerable<object[]> ShadowStatsTestData =>
    [
        [32, 54, 84, new Stats {CodeName = "", OverallRating = 16381.38m, CombatPower = 40824m, EvasionIndex  = 0.096m, StealthScore = 207m}],
        [9381, 130, 81, new Stats {CodeName = "", OverallRating = 37996.97m, CombatPower = 94770m, EvasionIndex  = 28.143m, StealthScore = 316.5m}],
        [85, 1, 94, new Stats {CodeName = "", OverallRating = 374.11m, CombatPower = 846m, EvasionIndex  = 0.255m, StealthScore = 142.5m}],
    ];
    
    [Fact]
    public void Process_OverallRating_Rounds_Down_At_Midpoint()
    {
        // Arrange
        // Inputs are chosen to produce an unrounded OverallRating of exactly 2.135.
        // MidpointRounding.ToZero must truncate this to 2.13, not round up to 2.14.
        //
        //   combatPower  = 2×2×1×1      = 4.0  → 4.0  × 0.40 = 1.600
        //   evasionIndex = (100/1000)×1 = 0.1  → 0.1  × 0.35 = 0.035
        //   stealthScore = ((2+2)/2)×1  = 2.0  → 2.0  × 0.25 = 0.500
        //                                         total       = 2.135
        
        var shadow = new ShadowDto
        {
            Id = Guid.NewGuid(),
            CodeName = "Test",
            Clan = "RoundingClan",
            Origin = "Water",
            Rank = Rank.Danza,
            ShadowSkills = new ShadowDto.StealthMetricsDto
            {
                ShadowBlendScore       = 2,
                SilenceRating          = 2,
                AcrobaticsLevel        = AcrobaticsLevel.Beginner,
                InvisibilityDurationMs = 100,
            }
        };

        // Act
        var result = BattleStatsCalculator.Process(shadow);

        // Assert
        Assert.Equal(2.13m, result.OverallRating);
    }
}