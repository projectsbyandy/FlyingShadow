using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlyingShadow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User userRequest)
    {
        var result = _authenticationService.ValidateCredentials(userRequest);
        
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error);
    }
}