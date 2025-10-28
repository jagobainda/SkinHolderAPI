using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.External;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class ExternalController(IExternalLogic externalLogic) : ControllerBase
{
    private readonly IExternalLogic _externalLogic = externalLogic;

    [HttpPost("GetPlayerInfo")]
    public async Task<IActionResult> GetPlayerInfo([FromBody] string playerId)
    {
        return Ok(playerId);
        //return Ok(await _externalLogic.GetPlayerInfo(playerId));
    }
}
