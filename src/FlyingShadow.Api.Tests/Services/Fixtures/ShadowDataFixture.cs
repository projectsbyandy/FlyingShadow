using FlyingShadow.Api.Models.Ninja;

namespace FlyingShadow.Api.Tests.Services.Fixtures;

public class ShadowDataFixture : IDisposable
{
    public readonly IList<Shadow> Shadows;
    public readonly IList<StealthMetrics>  StealthMetrics;

    public ShadowDataFixture()
    {
        Shadows = new List<Shadow>()
        {
            new()
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000034"),
                Clan = "Hidden Sound",
                CodeName = "Shadow Dragon",
                Origin = "Land of Sand",
                Rank = Rank.Toshiyama
            },
            new()
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000035"),
                Clan = "Shadow Hawk III",
                CodeName = "Shadow Hawk III",
                Origin = "Land of Wind",
                Rank = Rank.Oniwaban
            },
            new()
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000036"),
                Clan = "Seven Swordsmen",
                CodeName = "Shadow Viper",
                Origin = "Mist Country",
                Rank = Rank.Danza
            }
        };

        StealthMetrics = new List<StealthMetrics>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                ShadowId = Guid.Parse("550e8400-e29b-41d4-a716-000000000034"),
                ShadowBlendScore = 41,
                SilenceRating = 75,
                InvisibilityDurationMs = 2996,
                AcrobaticsLevel = AcrobaticsLevel.Intermediate
            },
            new()
            {
                Id = Guid.NewGuid(),
                ShadowId = Guid.Parse("550e8400-e29b-41d4-a716-000000000035"),
                ShadowBlendScore = 55,
                SilenceRating = 48,
                InvisibilityDurationMs = 1022,
                AcrobaticsLevel = AcrobaticsLevel.Beginner
            },
            new()
            {
                Id = Guid.NewGuid(),
                ShadowId = Guid.Parse("550e8400-e29b-41d4-a716-000000000036"),
                ShadowBlendScore = 11,
                SilenceRating = 49,
                InvisibilityDurationMs = 3714,
                AcrobaticsLevel = AcrobaticsLevel.Advanced
            }
        };
    }
    
    public void Dispose()
    {
        Shadows.Clear();
        StealthMetrics.Clear();
    }
}