using FlyingShadow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlyingShadow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlyingShadowController : ControllerBase
{
    private readonly IShadowService _shadowService;

    public FlyingShadowController(IShadowService shadowService)
    {
        _shadowService = shadowService;
    }

    [HttpGet("Shadows")]

    public IActionResult GetShadows()
    {
        var result = _shadowService.GetAllShadowDetails();
        return result.IsSuccess
            ? Ok(result.Value)
            :BadRequest(result.Error);
    }
}