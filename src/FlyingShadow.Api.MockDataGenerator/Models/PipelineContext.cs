namespace FlyingShadow.Api.MockDataGenerator.Models;

internal record PipelineContext(
    MockDataOptions MockDataOptions,
    string JwtKey,
    IReadOnlyList<UserCredentials> Credentials
);