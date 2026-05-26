using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services;

public interface IAuthenticationService
{
    public Task<Result<UserDto, Error>> ValidateCredentialsAsync(LoginDetails request);
    public Task<Result<UserDto, Error>> RegisterAsync(RegisterRequest request);
}