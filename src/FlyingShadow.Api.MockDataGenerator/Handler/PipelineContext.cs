namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal record PipelineContext(
    FakeDataDestinationPaths FakeDataDestinationPaths,
    string JwtKey,
    IReadOnlyList<UserCredentials> Credentials
);

internal record FakeDataDestinationPaths(
    string JwtKeyPath,
    string LoginDetailsListPath,
    string UsersPath,
    string ShadowsPath,
    string StealthMetricsPath);

internal record UserCredentials(
    Guid UserId,
    string Email,
    string Password,
    string HashedPassword
);

