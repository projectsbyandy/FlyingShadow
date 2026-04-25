namespace FlyingShadow.Api.MockDataGenerator.Models;

internal record PipelineContext(
    FakeDataDestinationPaths FakeDataDestinationPaths,
    string JwtKey,
    IReadOnlyList<UserCredentials> Credentials
);