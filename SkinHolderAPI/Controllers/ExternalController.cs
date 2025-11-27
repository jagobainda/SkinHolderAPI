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

        var result = await _externalLogic.GetPlayerInfoAsync(playerId);

        if (string.IsNullOrEmpty(result)) return Forbid();

        return Ok(result);
    }

    [HttpPost("GetFaceitCompleteData")]
    [Limit(30)]
    [AllowAnonymous]
    public async Task<IActionResult> GetFaceitCompleteData([FromBody] string steamId)
    {
        if (string.IsNullOrEmpty(steamId)) return BadRequest("Steam ID is required");

        var (playerData, playerId) = await _externalLogic.GetFaceitPlayerDataAsync(steamId);

        if (string.IsNullOrEmpty(playerData) || string.IsNullOrEmpty(playerId)) return Forbid();

        var csgoStatsTask = _externalLogic.GetFaceitPlayerStatsAsync(playerId, "csgo");
        var cs2StatsTask = _externalLogic.GetFaceitPlayerStatsAsync(playerId, "cs2");
        var bansTask = _externalLogic.GetFaceitBansAsync(playerId);

        var results = await Task.WhenAll(csgoStatsTask, cs2StatsTask, bansTask);

        var combinedData = new
        {
            playerData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(playerData),
            stats = new
            {
                csgo = string.IsNullOrEmpty(results[0]) ? (object?)null : System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(results[0]),
                cs2 = string.IsNullOrEmpty(results[1]) ? (object?)null : System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(results[1])
            },
            bans = string.IsNullOrEmpty(results[2]) ? (object?)null : System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(results[2])
        };

        return Ok(combinedData);
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
        var result = await _externalLogic.GetGamerPayPricesAsync();

        if (string.IsNullOrEmpty(result)) return NoContent();

        return Ok(result);
    }

    [HttpPost("GetSteamPrice")]
    [Limit(30)]
    [Authorize]
    public async Task<IActionResult> GetSteamPrice([FromBody] string steamHashName)
    {
        var result = await _externalLogic.GetSteamPriceAsync(steamHashName);

        if (string.IsNullOrEmpty(result)) return NoContent();

        return Ok(result);
    }
}