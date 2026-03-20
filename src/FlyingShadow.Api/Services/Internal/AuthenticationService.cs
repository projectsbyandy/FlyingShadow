using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.ResultType;
using FlyingShadow.Api.Repositories;

namespace FlyingShadow.Api.Services.Internal;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public Result<User, Error> ValidateCredentials(User user)
    {
        return _userRepository.GetUser(user.Email)
            .Bind(dbUser => VerifyPassword(user, dbUser.Hash));
    }

    private Result<User, Error> VerifyPassword(User user, string dbUserPasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(user.Password, dbUserPasswordHash) 
            ? Result<User, Error>.Success(user) 
            : Result<User, Error>.Failure(new Error("INVALID_CREDENTIALS", $"The email or password provided is incorrect")); 
    }
}