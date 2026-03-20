using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.ResultType;

namespace FlyingShadow.Api.Services;

public interface IAuthenticationService
{
    public Result<User, Error> ValidateCredentials(User user);
}