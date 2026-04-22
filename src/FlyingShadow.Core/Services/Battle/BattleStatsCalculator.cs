using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Battle;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Services.Battle;

public static class BattleStatsCalculator
{
    public static Stats Process(ShadowDto s)
    {
        var acrobaticsMultiplier = s.ShadowSkills.AcrobaticsLevel.ToMultiplier();
        var rankMultiplier = s.Rank.ToMultiplier();
        
        var combatPower  = (decimal)s.ShadowSkills.ShadowBlendScore * s.ShadowSkills.SilenceRating * acrobaticsMultiplier * rankMultiplier;
        var evasionIndex = s.ShadowSkills.InvisibilityDurationMs / 1000.0m * acrobaticsMultiplier;
        var stealthScore = (s.ShadowSkills.ShadowBlendScore + s.ShadowSkills.SilenceRating) / 2.0m * acrobaticsMultiplier;
        var overall      = Math.Round(combatPower * 0.4m + evasionIndex * 0.35m + stealthScore * 0.25m, 2, MidpointRounding.ToZero);

        return new Stats
        {
            CodeName =  s.CodeName,
            CombatPower   = combatPower,
            EvasionIndex  = evasionIndex,
            StealthScore  = stealthScore,
            OverallRating = overall
        };
    }
}