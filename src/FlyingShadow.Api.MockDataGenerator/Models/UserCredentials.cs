namespace FlyingShadow.Api.MockDataGenerator.Models;

internal record UserCredentials(
    Guid UserId,
    string Email,
    string Password,
    string HashedPassword
);