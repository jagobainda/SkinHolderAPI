using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Items;
using SkinHolderAPI.Attributes;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ItemsController(IItemsLogic itemsLogic) : ControllerBase
{
    public readonly IItemsLogic _itemsLogic = itemsLogic;

    [HttpGet]
    [Limit(30)]
    public async Task<IActionResult> Get()
    {
        var items = await _itemsLogic.GetItemsAsync();
        
        return Ok(items);
    }
}
