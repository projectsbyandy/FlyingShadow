using FlyingShadow.Api.DTO.Authenticate;

namespace FlyingShadow.Api.Services;

public interface IAuthenticationService
{
    public bool ValidateCredentials(User user);
}