namespace FlyingShadow.Core.Models.Battle;

public record Stats
{
    public required string CodeName { get; init; }
    public required decimal CombatPower { get; init; }
    public required decimal EvasionIndex { get; init; }
    public required decimal StealthScore { get; init; }
    public required decimal OverallRating { get; init; }
}