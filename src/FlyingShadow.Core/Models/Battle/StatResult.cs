namespace FlyingShadow.Core.Models.Battle;

public record StatResults
{
    public required string CombatPowerWinner { get; init; }
    public required string EvasionIndexWinner { get; init; }
    public required string StealthScoreWinner { get; init; }
}