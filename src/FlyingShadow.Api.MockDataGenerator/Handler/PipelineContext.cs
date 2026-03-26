namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal record PipelineContext(
    FakeDataDestinationPaths FakeDataDestinationPaths,
    string JwtSecret,
    IReadOnlyList<UserCredentials> Credentials
);

internal record FakeDataDestinationPaths(
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

