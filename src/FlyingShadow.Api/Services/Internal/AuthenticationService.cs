using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.Repositories;

namespace FlyingShadow.Api.Services.Internal;

internal class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public bool ValidateCredentials(User user)
    {
        var result = _userRepository.GetUser(user.Email);
        
        if (result.IsSuccess is false && result.Value is null )
            return false;
        
        return BCrypt.Net.BCrypt.Verify(user.Password, result.Value?.Hash);
    }
}