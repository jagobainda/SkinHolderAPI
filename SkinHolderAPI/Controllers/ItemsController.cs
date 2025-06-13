using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Items;
using SkinHolderAPI.Attributes;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController(IItemsLogic itemsLogic) : ControllerBase
{
    public readonly IItemsLogic _itemsLogic = itemsLogic;

    [HttpGet]
    [Authorize]
    [Limit(5)]
    public async Task<IActionResult> Get()
    {
        var items = await _itemsLogic.GetItemsAsync();

        if (items.Count == 0) return NotFound();
        
        return Ok(items);
    }
}
