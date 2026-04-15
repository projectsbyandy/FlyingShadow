using FlyingShadow.Core.DTO.Battle;

namespace FlyingShadow.Core.Models.Battle;

public record BattleReport
{
    public required string Outcome { get; init; }
    public required Stats ShadowOneStats { get; init; }
    public required Stats ShadowTwoStats { get; init; }
    public required StatResults StatBreakdown { get; init; }

    public BattleResponse ToBattleResponse()
    {
        return new BattleResponse()
        {
            Outcome = Outcome.Equals("draw", StringComparison.CurrentCultureIgnoreCase) ? "Draw!" : $"{Outcome} Wins!",
            ShadowOneStats = ShadowOneStats,
            ShadowTwoStats = ShadowTwoStats,
            StatBreakdown = StatBreakdown
        };
    }
}