using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Battle;

namespace FlyingShadow.Core.Services.Battle;

public static class BattleStatsCalculator
{
    public static Stats Process(ShadowDto s)
    {
        var acrobaticsMultiplier = (int)s.ShadowSkills.AcrobaticsLevel;
        var rankMultiplier = (int)s.Rank;

        var combatPower  = s.ShadowSkills.ShadowBlendScore * s.ShadowSkills.SilenceRating * acrobaticsMultiplier * rankMultiplier;
        var evasionIndex = (s.ShadowSkills.InvisibilityDurationMs / 1000.0) * acrobaticsMultiplier;
        var stealthScore = ((s.ShadowSkills.ShadowBlendScore + s.ShadowSkills.SilenceRating) / 2.0) * acrobaticsMultiplier;
        var overall      = Math.Round((combatPower * 0.4) + (evasionIndex * 0.35) + (stealthScore * 0.25), 2, MidpointRounding.ToZero);

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