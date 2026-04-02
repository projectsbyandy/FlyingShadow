using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<LoginResponse> Login([FromBody] LoginDetails user)
    {
        var tokenResult = _authenticationService.ValidateCredentials(user)
            .Bind(userDto => _tokenService.GenerateToken(userDto.UserId, userDto.Email));
        
        return tokenResult.IsSuccess 
            ? Ok(new LoginResponse(tokenResult.Value))
            : Unauthorized(new ErrorResponse("Invalid Email or Password"));
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<RegisterResponse> Register([FromBody] RegisterRequest request)
    {
        var result = _authenticationService.Register(request);

        return result.IsSuccess
            ? Created(string.Empty, new RegisterResponse(result.Value.UserId))
            : MapError(result.Error);
    }

    private ActionResult MapError(Error error) => error.Code switch
    {
        ErrorCode.NotFound or
        ErrorCode.AlreadyExists => BadRequest(new ErrorResponse("Registration could not be completed.")),
        _ => StatusCode(500, new ErrorResponse("An unexpected error occurred."))
    };
}