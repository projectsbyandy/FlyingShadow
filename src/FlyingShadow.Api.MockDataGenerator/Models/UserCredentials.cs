using FlyingShadow.Core.DTO.Authenticate;

namespace FlyingShadow.Api.MockDataGenerator.Models;

internal record UserCredentials(
    Guid UserId,
    string Email,
    string Password,
    string HashedPassword
)
{
    public LoginDetails ToLoginDetails()
    {
        return new LoginDetails()
        {
            Email = Email,
            Password = Password
        };
    }
}