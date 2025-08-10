using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.UserItems;
using SkinHolderAPI.Attributes;
using System.Security.Claims;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserItemsController(IUserItemsLogic userItemsLogic) : ControllerBase
{
    public readonly IUserItemsLogic _userItemsLogic = userItemsLogic;

    [HttpGet]
    [Limit(5)]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var userItems = await _userItemsLogic.GetUserItemsAsync(userId);

        if (userItems.Count == 0) return NotFound("No user items found."); 

        return Ok(userItems);
    }
}
