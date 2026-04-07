using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Services;
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
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public ActionResult<IList<ShadowDto>> GetShadows()
    {
        var result = _shadowService.GetAllShadowDetails();
        
        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(500, new ErrorResponse(result.Error.Message));
    }
}