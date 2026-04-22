namespace FlyingShadow.Core.Models.Ninja;

internal static class RankExtensions
{
    public static decimal ToMultiplier(this Rank rank) => rank switch
    {
        Rank.Danza   => 1m,
        Rank.Toshiyama  => 2m,
        Rank.Oniwaban   => 3m,
        _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, "Unknown rank")
    };
}