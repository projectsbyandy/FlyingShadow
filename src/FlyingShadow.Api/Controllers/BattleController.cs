using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.Services.Battle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyingShadow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BattleController : ControllerBase
{
    private readonly IBattleService _battleService;

    public BattleController(IBattleService battleService)
    {
        _battleService = battleService;
    }

    [HttpPost("start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<BattleResponse> Battle([FromBody] BattleRequest request)
    {
       var result = _battleService.Battle(request.ShadowOneCodeName, request.ShadowTwoCodeName);
       
       if (result.IsSuccess)
           return Ok(result.Value);
       
       return BadRequest(result.Error);
    }
}