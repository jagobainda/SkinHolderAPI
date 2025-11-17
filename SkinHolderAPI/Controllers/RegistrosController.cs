using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Registros;
using SkinHolderAPI.Attributes;
using SkinHolderAPI.DTOs.Registros;
using System.Security.Claims;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class RegistrosController(IRegistrosLogic registrosLogic) : ControllerBase
{
    private readonly IRegistrosLogic _registrosLogic = registrosLogic;

    [HttpGet("GetLastRegistro")]
    [Limit(5)]
    public async Task<IActionResult> GetLastRegistro()
    {
        var userId = GetUserId();

        var registro = await _registrosLogic.GetLastRegistroAsync(userId);

        if (registro == null) return BadRequest();

        return Ok(registro);
    }

    [HttpGet]
    [Limit(5)]
    public async Task<IActionResult> Get()
    {
        var userId = GetUserId();

        var registros = await _registrosLogic.GetRegistrosAsync(userId);

        if (registros.Count == 0) return NoContent();

        return Ok(registros);
    }

    [HttpPost]
    [Limit(5)]
    public async Task<IActionResult> Create([FromBody] RegistroDto registroDto)
    {
        if (registroDto == null) return BadRequest("Registro cannot be null");

        var userId = GetUserId();

        if (userId != registroDto.Userid) return BadRequest("User ID mismatch.");

        if (registroDto == null) return BadRequest("Registro cannot be null");

        var registroId = await _registrosLogic.CreateRegistroAsync(registroDto);

        return registroId != 0 ? Ok(registroId) : BadRequest("Failed to create registro.");
    }

    [HttpDelete]
    [Limit(5)]
    public async Task<IActionResult> Delete(long registroId)
    {
        if (registroId < 1) return BadRequest("Invalid registroId.");

        var userId = GetUserId();

        var registro = await _registrosLogic.GetRegistroAsync(registroId);

        if (registro == null) return NotFound("Registro not found.");

        if (registro.Userid != userId) return Forbid("You do not have permission to delete this registro.");

        var success = await _registrosLogic.DeleteRegistroAsync(registroId);

        return success ? Ok("Registro deleted successfully.") : BadRequest("Failed to delete registro.");
    }

    [HttpGet("GetVarianceStats")]
    [Limit(10)]
    public async Task<IActionResult> GetVarianceStats()
    {
        var userId = GetUserId();

        var stats = await _registrosLogic.GetVarianceStatsAsync(userId);

        if (stats == null) return BadRequest();

        return Ok(stats);
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}
