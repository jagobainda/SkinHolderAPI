using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Users;
using SkinHolderAPI.Attributes;
using SkinHolderAPI.DTOs.Login;
using System.Security.Claims;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AuthController(IUserLogic userLogic) : ControllerBase
{
    private readonly IUserLogic _userLogic = userLogic;


    [HttpPost("login")]
    [Limit(15)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _userLogic.LoginAsync(dto);

        if (result == null) return Unauthorized();

        return Ok(result);
    }

    [HttpGet("validate")]
    [Limit(20)]
    public IActionResult ValidateToken()
    {
        return Ok(new
        {
            valid = true,
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            username = User.FindFirst(ClaimTypes.Name)?.Value
        });
    }

    [HttpPost("requestAccess")]
    [Limit(1)]
    [AllowAnonymous]
    public IActionResult RequestAccess([FromBody] string email)
    {
        // TODO: Implement access request logic
        return Ok();
    }
}