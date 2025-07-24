using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Apis;
using SkinHolderAPI.Attributes;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ApiQueryController(IApiQueryLogic apiQueryLogic) : ControllerBase
{
    private readonly IApiQueryLogic _apiQueryLogic = apiQueryLogic;

    [HttpPost("GetGamerPayPrices")]
    [Limit(1)]
    public async Task<IActionResult> GetGamerPayPrices([FromBody] List<string> itemNames)
    {
        if (itemNames == null || itemNames.Count == 0) return BadRequest("Item names cannot be null or empty.");

        var prices = await _apiQueryLogic.GetGamerPayItemPricesAsync(itemNames);

        return Ok(prices);
    }

    [HttpPost("GetCSFloatPrices")]
    [Limit(1)]
    public async Task<IActionResult> GetCSFloatPrices([FromBody] List<string> itemNames)
    {
        if (itemNames == null || itemNames.Count == 0) return BadRequest("Item names cannot be null or empty.");

        var prices = await _apiQueryLogic.GetCSFloatItemPricesAsync(itemNames);

        return Ok(prices);
    }
}
