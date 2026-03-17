namespace FlyingShadow.Api.DTO.Ninja;

internal record StealthMetrics
{
    public Guid ShadowId { get; set; }
    public int ShadowBlendScore { get; set; }
    public int SilenceRating { get; set; }
    public TimeSpan InvisibilityDuration { get; set; }
    public AcrobaticsLevel AcrobaticsLevel { get; set; }
}