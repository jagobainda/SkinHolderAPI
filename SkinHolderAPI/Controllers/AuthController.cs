using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Users;
using SkinHolderAPI.Attributes;
using SkinHolderAPI.DTOs.Login;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AuthController(IUserLogic userLogic) : ControllerBase
{
    private readonly IUserLogic _userLogic = userLogic;


    [HttpPost("login")]
    [Limit(5)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _userLogic.LoginAsync(dto);

        if (result == null) return Unauthorized();

        return Ok(result);
    }
}