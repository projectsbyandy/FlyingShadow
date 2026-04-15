using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Services.Mappers;

namespace FlyingShadow.Api.Tests.Fixtures;

public class ShadowDataFixture : IDisposable
{
    public readonly IList<Shadow> Shadows = new List<Shadow>()
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
    
    public readonly IList<StealthMetrics> StealthMetrics = new List<StealthMetrics>()
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

    protected IList<ShadowDto> GetShadowDTOs()
    {
        var shadowMapper = new ShadowDtoMapper();
        var metricsById = StealthMetrics.ToDictionary(m => m.ShadowId);

        return Shadows
            .Where(s => metricsById.ContainsKey(s.Id))
            .Select(s => shadowMapper.ToSingle(s, metricsById[s.Id]))
            .ToList();   
    }
    
    public void Dispose()
    {
        Shadows.Clear();
        StealthMetrics.Clear();
    }
}