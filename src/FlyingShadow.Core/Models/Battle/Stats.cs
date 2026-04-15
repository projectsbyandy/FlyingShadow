namespace FlyingShadow.Core.Models.Battle;

public record Stats
{
    public required string CodeName { get; set; }
    public required double CombatPower { get; init; }
    public required double EvasionIndex { get; init; }
    public required double StealthScore { get; init; }
    public required double OverallRating { get; init; }
}