namespace FlyingShadow.Api.MockDataGenerator.Models;

internal record FakeDataDestinationPaths(
    string JwtKeyPath,
    string LoginDetailsListPath,
    string UsersPath,
    string ShadowsPath,
    string StealthMetricsPath);