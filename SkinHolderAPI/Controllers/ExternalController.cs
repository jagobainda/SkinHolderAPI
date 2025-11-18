using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.External;
using SkinHolderAPI.Attributes;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ExternalController(IExternalLogic externalLogic) : ControllerBase
{
    private readonly IExternalLogic _externalLogic = externalLogic;

    [HttpPost("GetPlayerInfo")]
    [Limit(30)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlayerInfo([FromBody] string playerId)
    {
        if (playerId.Length > 25) return BadRequest("Wrong id format");

        var result =  await _externalLogic.GetPlayerInfoAsync(playerId);

        if (string.IsNullOrEmpty(result)) return Forbid();
        
        return Ok(result);
    }

    [HttpGet("GetExtensionUsage")]
    [Limit(5)]
    [AllowAnonymous]
    public async Task<IActionResult> GetExtensionUsage()
    {
        var usage = await _externalLogic.GetExtensionUsageAsync();

        if (usage == null) return NotFound("No data available");

        return Ok(usage);
    }

    [HttpGet("GetGamerPayPrices")]
    [Limit(5)]
    [Authorize]
    public async Task<IActionResult> GetGamerPayPrices()
    {
        return Ok(await _externalLogic.GetGamerPayPricesAsync());
    }
}
