using Ardalis.GuardClauses;
using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;
using FlyingShadow.Core.Utils;

namespace FlyingShadow.Api.Services;

internal class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthenticationService(IUserRepository userRepository, IPasswordHasher passwordHasher) 
    {
        _passwordHasher = passwordHasher; 
        _userRepository = userRepository;
    }
        
    public async Task<Result<UserDto, Error>> ValidateCredentialsAsync(LoginDetails request)
    {
        return await _userRepository.GetUserAsync(request.Email)
            .Bind(user => VerifyPassword(request, user));
    }

    public async Task<Result<UserDto, Error>> RegisterAsync(RegisterRequest registerRequest)
    {
        try
        {
            return await _userRepository.EnsureUserDoesNotExistAsync(registerRequest.Email)
                .BindAsync(_ => _userRepository.AddUserAsync(new User()
                {
                    Email = registerRequest.Email,
                    HashedPassword = _passwordHasher.Hash(registerRequest.Password)
                }))
                .Bind(user => Result<UserDto, Error>.Success(user.ToDto()));
        }
        catch (Exception ex)
        {
            return Result<UserDto, Error>.Failure(new Error(ErrorCode.UnexpectedError, $"Unable to Register User due to: {ex.Message}"));
        }
    }

    private Result<UserDto, Error> VerifyPassword(LoginDetails request, User user)
    {
        try
        {
            return _passwordHasher.Verify(request.Password, Guard.Against.NullOrEmpty(user.HashedPassword))
                ? Result<UserDto, Error>.Success(user.ToDto())
                : Result<UserDto, Error>.Failure(new Error(ErrorCode.InvalidCredentials, "The email or password provided is incorrect"));
        }
        catch (Exception ex)
        {
            return Result<UserDto, Error>.Failure(new Error(ErrorCode.UnexpectedError, ex.Message));
        }
    }
}