namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal record PipelineContext(
    string FakeUsersPath,
    string FakeDbUsersPath,
    string JwtSecret,
    IReadOnlyList<UserCredentials> Credentials
);
    
internal record UserCredentials(
    string UserId,
    string Email,
    string Password,
    string Hash
);

