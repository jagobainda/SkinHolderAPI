using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Registros;
using SkinHolderAPI.Attributes;
using System.Security.Claims;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class RegistrosController(IRegistrosLogic registrosLogic) : ControllerBase
{
    private readonly IRegistrosLogic _registrosLogic = registrosLogic;

    [HttpGet]
    [Limit(5)]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var registros = await _registrosLogic.GetRegistrosAsync(userId);

        return Ok(registros);
    }

    [HttpGet("GetLastRegistro")]
    [Limit(5)]
    public async Task<IActionResult> GetLastRegistro()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var registro = await _registrosLogic.GetLastRegistroAsync(userId);

        return Ok(registro);
    }
}
