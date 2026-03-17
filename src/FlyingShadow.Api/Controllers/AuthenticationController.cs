using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlyingShadow.Api.Controllers;

[Controller]
internal class AuthenticationController
{
    private readonly IAuthenticationService _authenticationService;
    
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User userRequest)
    {
        throw new NotImplementedException();
    }
}