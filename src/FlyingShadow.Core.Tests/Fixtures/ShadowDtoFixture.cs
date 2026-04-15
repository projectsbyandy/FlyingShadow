using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Tests.Fixtures;

public class ShadowDtoFixture
{
    protected readonly ShadowDto ShadowOne = new()
    {
        Id = Guid.NewGuid(),
        CodeName = "Fleeting Spirit",
        Clan = "Underground Dweller",
        Origin = "Lakeside",
        Rank = Rank.Oniwaban,
        ShadowSkills = new()
        {
            AcrobaticsLevel = AcrobaticsLevel.Advanced,
            InvisibilityDurationMs = 9131,
            ShadowBlendScore = 88,
            SilenceRating = 13
        }
    };

    protected readonly ShadowDto ShadowTwo = new()
    {
        Id = Guid.NewGuid(),
        CodeName = "Peeking Fish",
        Clan = "Water Dweller",
        Origin = "Freshwater",
        Rank = Rank.Danza,
        ShadowSkills = new()
        {
            AcrobaticsLevel = AcrobaticsLevel.Intermediate,
            InvisibilityDurationMs = 931,
            ShadowBlendScore = 23,
            SilenceRating = 100
        }
    };
}