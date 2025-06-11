using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Users;
using SkinHolderAPI.DTOs.Login;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserLogic userLogic) : ControllerBase
{
    private readonly IUserLogic _userLogic = userLogic;


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _userLogic.LoginAsync(dto);

        if (result == null) return Unauthorized();

        return Ok(result);
    }
}