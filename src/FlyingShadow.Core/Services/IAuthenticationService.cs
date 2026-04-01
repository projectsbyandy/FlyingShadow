using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services;

public interface IAuthenticationService
{
    public Result<UserDto, Error> ValidateCredentials(LoginDetails request);
    public Result<UserDto, Error>Register(RegisterRequest request);
}