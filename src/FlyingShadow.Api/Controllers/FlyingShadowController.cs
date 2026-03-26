using FlyingShadow.Api.Services.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyingShadow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlyingShadowController : ControllerBase
{
    private readonly ShadowService _shadowService;

    public FlyingShadowController(ShadowService shadowService)
    {
        _shadowService = shadowService;
    }

    [HttpGet("Shadows")]
    [Authorize]
    public IActionResult GetShadows()
    {
        return Ok();
    }
}