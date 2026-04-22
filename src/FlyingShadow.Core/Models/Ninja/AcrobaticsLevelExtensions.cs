namespace FlyingShadow.Core.Models.Ninja;

internal static class AcrobaticsLevelExtensions
{
    public static decimal ToMultiplier(this AcrobaticsLevel level) => level switch
    {
        AcrobaticsLevel.Beginner => 1m,
        AcrobaticsLevel.Intermediate => 2m,
        AcrobaticsLevel.Advanced => 3m,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unknown acrobatics level")
    };
}