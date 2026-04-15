using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Services.Battle;
using FlyingShadow.Core.Tests.Fixtures;

namespace FlyingShadow.Core.Tests.Services.Battle;

public class BattleProcessorTests : ShadowDtoFixture
{
    private readonly IBattleProcessor _sut;
    
    public BattleProcessorTests()
    {
        _sut = new BattleProcessor();
    }

    [Fact]
    public void Verify_Processed_ShadowDTOs_Returns_Success()
    {
        // Act
        var result = _sut.Process(ShadowOne, ShadowTwo);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
    
    [Fact]
    public void Verify_Processed_ShadowDTOs_Returns_The_Correct_Stats_Winners()
    {
        // Arrange / Act
        var result = _sut.Process(ShadowOne, ShadowTwo);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ShadowOne.CodeName, result.Value.StatBreakdown.CombatPowerWinner);
        Assert.Equal(ShadowOne.CodeName, result.Value.StatBreakdown.EvasionIndexWinner);
        Assert.Equal(ShadowOne.CodeName, result.Value.StatBreakdown.StealthScoreWinner);
    }
    
    [Fact]
    public void Verify_Processed_ShadowDTOs_Returns_Draw_Stats()
    {
        // Arrange
        var updatedShadowOne = ShadowOne with {Rank = Rank.Danza, ShadowSkills = new() { AcrobaticsLevel =  AcrobaticsLevel.Advanced, InvisibilityDurationMs = 1, ShadowBlendScore = 1, SilenceRating = 1 }};
        var updatedShadowTwo = ShadowTwo with {Rank = Rank.Danza, ShadowSkills = new() { AcrobaticsLevel =  AcrobaticsLevel.Advanced, InvisibilityDurationMs = 1, ShadowBlendScore = 1, SilenceRating = 1 }};
        
        // Act
        var result = _sut.Process(updatedShadowOne, updatedShadowTwo);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Draw", result.Value.StatBreakdown.CombatPowerWinner);
        Assert.Equal("Draw", result.Value.StatBreakdown.EvasionIndexWinner);
        Assert.Equal("Draw", result.Value.StatBreakdown.StealthScoreWinner);
    }
    
    [Theory]
    [InlineData(1, 1, "Draw!")]
    [InlineData(2, 1, "Fleeting Spirit Wins!")]
    [InlineData(1, 2, "Peeking Fish Wins!")]
    public void Verify_Processed_ShadowDTOs_Returns_The_Correct_Outcome_Report_Response(int shadowOneSkillsLevel, int shadowTwoSkillsLevel, string expectedOutcome)
    {
        // Arrange
        var updatedShadowOne = ShadowOne with
        {
            Rank = Rank.Oniwaban,
            ShadowSkills = new()
            {
                AcrobaticsLevel = AcrobaticsLevel.Advanced,
                InvisibilityDurationMs = shadowOneSkillsLevel,
                ShadowBlendScore = shadowOneSkillsLevel,
                SilenceRating = shadowOneSkillsLevel
            }
        };
        
        var updatedShadowTwo = ShadowTwo with
        {
            Rank = Rank.Oniwaban,
            ShadowSkills = new()
            {
                AcrobaticsLevel = AcrobaticsLevel.Advanced,
                InvisibilityDurationMs = shadowTwoSkillsLevel,
                ShadowBlendScore = shadowTwoSkillsLevel,
                SilenceRating = shadowTwoSkillsLevel
            }
        };
        
        // Act
        var result = _sut.Process(updatedShadowOne, updatedShadowTwo);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedOutcome, result.Value.Outcome);

    }
    
    [Fact]
    public void Verify_Shadows_Of_The_Same_Id_Cannot_Battle()
    {
        // Arrange
        var shadowTwoWithSameId= ShadowOne;

        // Act
        var result = _sut.Process(ShadowOne, shadowTwoWithSameId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(new Error(ErrorCode.UnableToProcessData, "Shadows with the same Id cannot battle"), result.Error);
    }
    
    [Fact]
    public void Verify_Shadows_Of_The_Same_Clan_Cannot_Battle()
    {
        // Arrange
        var shadowTwoWithSameClan= ShadowOne with { Id = Guid.NewGuid() };

        // Act
        var result = _sut.Process(ShadowOne, shadowTwoWithSameClan);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(new Error(ErrorCode.UnableToProcessData, "Shadows of the same clan cannot battle"), result.Error);
    }
}