using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Models.Users;
using FlyingShadow.Api.Repositories;

namespace FlyingShadow.Api.Services.Internal;

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
        return BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword) 
            ? Result<UserDto, Error>.Success(user.ToDto())
            : Result<UserDto, Error>.Failure(new Error("INVALID_CREDENTIALS", "The email or password provided is incorrect")); 
    }

    private string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 14);
}