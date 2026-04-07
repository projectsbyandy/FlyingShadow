using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Tests.DTO.Ninja;

public class ShadowMapperTests 
{
    [Fact]
    public void Successfully_Map_Shadow_And_Stealth_Metrics_To_DTO()
    {
        // Arrange
        var shadow = new Shadow
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000034"),
            Clan = "Hidden Sound",
            CodeName = "Shadow Dragon",
            Origin = "Land of Sand",
            Rank = Rank.Toshiyama
        };

        var stealthMetrics = new StealthMetrics()
        {
            Id = Guid.NewGuid(),
            ShadowId = Guid.Parse("550e8400-e29b-41d4-a716-000000000034"),
            ShadowBlendScore = 41,
            SilenceRating = 75,
            InvisibilityDurationMs = 2996,
            AcrobaticsLevel = AcrobaticsLevel.Intermediate
        };
        
        IShadowMapper sut = new ShadowMapper();
        
        // Act
        var mappedDto = sut.ToDto(shadow, stealthMetrics);
        
        // Assert
        Assert.Equal(shadow.Id, mappedDto.Id);
        Assert.Equal(shadow.Clan, mappedDto.Clan);
        Assert.Equal(shadow.CodeName, mappedDto.CodeName);
        Assert.Equal(shadow.Origin, mappedDto.Origin);
        Assert.Equal(shadow.Rank, mappedDto.Rank);
        Assert.Equal(stealthMetrics.AcrobaticsLevel, mappedDto.ShadowSkills.AcrobaticsLevel);
        Assert.Equal(stealthMetrics.ShadowBlendScore, mappedDto.ShadowSkills.ShadowBlendScore);
        Assert.Equal(stealthMetrics.InvisibilityDurationMs, mappedDto.ShadowSkills.InvisibilityDurationMs);
        Assert.Equal(stealthMetrics.SilenceRating, mappedDto.ShadowSkills.SilenceRating);
    }
}