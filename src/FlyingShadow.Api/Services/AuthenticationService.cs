using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;

namespace FlyingShadow.Api.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public Result<UserDto, Error> ValidateCredentials(LoginDetails request)
    {
        return _userRepository.GetUser(request.Email)
            .Bind(user => VerifyPassword(request, user));
    }

    public Result<UserDto, Error> Register(RegisterRequest registerRequest)
    {
        return _userRepository.EnsureUserDoesNotExist(registerRequest.Email)
            .Bind(_ => _userRepository.AddUser(new User()
            {
                Email = registerRequest.Email,
                HashedPassword = HashPassword(registerRequest.Password)
            }))
            .Bind(user => Result<UserDto, Error>.Success(user.ToDto()));
    }

    private Result<UserDto, Error> VerifyPassword(LoginDetails request, User user)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword) 
                ? Result<UserDto, Error>.Success(user.ToDto())
                : Result<UserDto, Error>.Failure(new Error("INVALID_CREDENTIALS", "The email or password provided is incorrect"));
        }
        catch (Exception ex)
        {
            return Result<UserDto, Error>.Failure(new  Error("UNABLE_TO_VALIDATE", ex.Message));
        }
    }

    private string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 14);
}