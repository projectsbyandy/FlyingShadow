using FlyingShadow.Api.Models.Ninja;

namespace FlyingShadow.Api.DTO.Ninja;

public record ShadowDto
{
    public required string CodeName { get; init; }
    public required string Clan { get; init; }
    public required string Origin { get; init; }
    public required Rank Rank { get; init; }
    public required StealthMetricsDto ShadowSkills { get; init; } = new();
    
    public record StealthMetricsDto
    {
        public int ShadowBlendScore { get; init; }
        public int SilenceRating { get; init; }
        public TimeSpan InvisibilityDuration { get; init; }
        public AcrobaticsLevel AcrobaticsLevel { get; init; }
    }
}