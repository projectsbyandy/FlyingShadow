using FlyingShadow.Api.DTO.Shadow;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyingShadow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlyingShadowController : ControllerBase
{
    private readonly IShadowService _shadowService;

    public FlyingShadowController(IShadowService shadowService)
    {
        _shadowService = shadowService;
    }

    [HttpGet("Shadows")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<ShadowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public IActionResult GetShadows()
    {
        var result = _shadowService.GetAllShadowDetails();
        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(500, result.Error);
    }
}