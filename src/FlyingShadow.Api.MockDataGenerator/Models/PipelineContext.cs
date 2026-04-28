namespace FlyingShadow.Api.MockDataGenerator.Models;

internal record PipelineContext(
    MockDataOptions MockDataOptions,
    string JwtKey,
    IReadOnlyList<UserCredentials> Credentials
)
{
    internal string StaticShadowData { get; set; } = Path.Combine(AppContext.BaseDirectory, "./StaticData/shadows.json");
    internal string StaticStealthMetricsData { get; set; } = Path.Combine(AppContext.BaseDirectory, "./StaticData/stealthMetrics.json");
}