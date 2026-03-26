namespace FlyingShadow.Api.Models.Ninja;

public record StealthMetrics
{
    public Guid Id { get; set; }
    public Guid ShadowId { get; set; }
    public int ShadowBlendScore { get; set; }
    public int SilenceRating { get; set; }
    public int InvisibilityDurationMs { get; set; }
    public AcrobaticsLevel AcrobaticsLevel { get; set; }
}