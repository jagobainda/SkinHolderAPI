using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Users;
using SkinHolderAPI.Attributes;
using SkinHolderAPI.DTOs.Users;
using System.Security.Claims;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserSettingsController(IUserLogic userLogic) : ControllerBase
{
    private readonly IUserLogic _userLogic = userLogic;

    [HttpGet]
    [Limit(10)]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var userInfo = await _userLogic.GetUserInfoAsync(userId);

        if (userInfo == null) return NotFound("Usuario no encontrado.");

        return Ok(userInfo);
    }

    [HttpPut("password")]
    [Limit(5)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var (success, errorMessage) = await _userLogic.ChangePasswordAsync(userId, dto);

        if (!success) return BadRequest(errorMessage);

        return Ok("Contraseña actualizada correctamente.");
    }

    [HttpDelete("account")]
    [Limit(3)]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var (success, errorMessage) = await _userLogic.DeleteAccountAsync(userId, dto);

        if (!success) return BadRequest(errorMessage);

        return Ok("Cuenta eliminada correctamente.");
    }
}
