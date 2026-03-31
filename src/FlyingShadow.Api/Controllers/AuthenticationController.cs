using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.Services;
using FlyingShadow.Core.Models.ResultType;
using Microsoft.AspNetCore.Mvc;

namespace FlyingShadow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenService _tokenService;
    
    public AuthenticationController(IAuthenticationService authenticationService, ITokenService tokenService)
    {
        _authenticationService = authenticationService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDetails user)
    {
        var tokenResult = _authenticationService.ValidateCredentials(user)
            .Bind(userDto => _tokenService.GenerateToken(userDto.UserId, userDto.Email));
        
        return tokenResult.IsSuccess
            ? Ok(new { token = tokenResult.Value })
            : Unauthorized("Unauthorized Email or Password");
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var result = _authenticationService.Register(request);
        return result.IsSuccess
            ? Ok(new { token = result.Value })
            : BadRequest(result.Error);
    }
}