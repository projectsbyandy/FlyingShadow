using FlyingShadow.Core.Models.Battle;

namespace FlyingShadow.Core.DTO.Battle;

public record BattleResponse
{
    public required string Outcome { get; init; }
    public required Stats ShadowOneStats { get; set; }
    public required Stats ShadowTwoStats { get; set; }
    public required StatResults StatBreakdown { get; init; }
}