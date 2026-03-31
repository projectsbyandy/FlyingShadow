using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Services;

public interface IAuthenticationService
{
    public Result<UserDto, Error> ValidateCredentials(LoginDetails request);
    public Result<UserDto, Error>Register(RegisterRequest request);
}