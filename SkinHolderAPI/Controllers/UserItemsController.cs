using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.UserItems;
using SkinHolderAPI.Attributes;
using SkinHolderAPI.DTOs.UserItemsDto;
using System.Security.Claims;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserItemsController(IUserItemsLogic userItemsLogic) : ControllerBase
{
    public readonly IUserItemsLogic _userItemsLogic = userItemsLogic;

    [HttpGet]
    [Limit(15)]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var userItems = await _userItemsLogic.GetUserItemsAsync(userId);

        if (userItems.Count == 0) return NotFound("No user items found."); 

        return Ok(userItems);
    }

    [HttpPut]
    [Limit(40)]
    public async Task<IActionResult> Update([FromBody] UserItemDto userItems)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (userItems == null) return BadRequest("UserItem cannot be null");

        var success = await _userItemsLogic.UpdateUserItemAsync(userItems);

        return success ? Ok("User item updated successfully.") : BadRequest("Failed to update user item.");
    }
}
