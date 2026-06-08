using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Battle;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Services.Mappers;

namespace FlyingShadow.Api.Tests.Fixtures;

public class ShadowDataFixture : IDisposable
{
    public readonly IEnumerable<Shadow> Shadows = new List<Shadow>()
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
    
    public readonly IEnumerable<StealthMetrics> StealthMetrics = new List<StealthMetrics>()
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

    public IEnumerable<ShadowDto> GetShadowDTOs()
    {
        var shadowMapper = new ShadowDtoMapper();
        var metricsById = StealthMetrics.ToDictionary(m => m.ShadowId);

        return Shadows
            .Where(s => metricsById.ContainsKey(s.Id))
            .Select(s => shadowMapper.ToSingle(s, metricsById[s.Id]))
            .ToList();   
    }

    public readonly IList<User> Users = new List<User>()
    {
        new()
        {
            Email = "tim.h@horton.com",
            UserId = Guid.Parse("3beeba67-fdfb-4ed8-a470-f45327fc0c29"),
            HashedPassword = ""
        },
        new()
        {
            Email = "sally.lindle@horton.com",
            UserId = Guid.Parse("7bc9e0bf-d7aa-49eb-b825-44ff0c5496bb"),
            HashedPassword = ""
        },
        new()
        {
            Email = "greg.based@horton.com",
            UserId = Guid.Parse("0d771dae-d2a8-4299-849d-d09d7027aee8"),
            HashedPassword = ""
        }
    };

    public readonly BattleResponse BattleResponse = new()
    {
        Outcome = "test",
        ShadowOneStats = new Stats()
        {
            CodeName = "test",
            OverallRating = 1,
            CombatPower = 1,
            EvasionIndex = 1,
            StealthScore = 1
        },
        ShadowTwoStats = new Stats()
        {
            CodeName = "test",
            OverallRating = 1,
            CombatPower = 1,
            EvasionIndex = 1,
            StealthScore = 1
        },
        StatBreakdown = new StatResults()
        {
            CombatPowerWinner = "test",
            EvasionIndexWinner = "test",
            StealthScoreWinner = "test",
        }
    };

    public void Dispose()
    {
        Shadows.ToList().Clear();
        StealthMetrics.ToList().Clear();
        Users.ToList().Clear();
    }
}