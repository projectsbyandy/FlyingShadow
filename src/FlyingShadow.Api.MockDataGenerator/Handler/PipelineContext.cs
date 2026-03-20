namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal record PipelineContext(
    string FakeLoginDetailsListPath,
    string FakeUsersPath,
    string JwtSecret,
    IReadOnlyList<UserCredentials> Credentials
);
    
internal record UserCredentials(
    string UserId,
    string Email,
    string Password,
    string HashedPassword
);

