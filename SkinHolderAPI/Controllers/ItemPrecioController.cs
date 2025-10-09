using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.ItemPrecio;
using SkinHolderAPI.Application.Registros;
using SkinHolderAPI.Attributes;
using SkinHolderAPI.DTOs.ItemPrecio;
using SkinHolderAPI.Models;
using System.Security.Claims;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ItemPrecioController(IItemPrecioLogic itemPrecioLogic, IRegistrosLogic registrosLogic) : ControllerBase
{
    private readonly IItemPrecioLogic _itemPrecioLogic = itemPrecioLogic;
    private readonly IRegistrosLogic _registrosLogic = registrosLogic;

    [HttpGet("{registroId}")]
    [Limit(5)]
    public async Task<IActionResult> Get(long registroId)
    {
        if (registroId < 1) return BadRequest("Invalid registroId.");

        var registro = await _registrosLogic.GetRegistroAsync(registroId);

        var userId = GetUserId();

        if (registro == null) return BadRequest("Registro not found.");

        if (registro.Userid != userId) return Unauthorized("Unauthorized registro.");

        var itemPrecios = await _itemPrecioLogic.GetItemPreciosAsync(registroId);

        return Ok(itemPrecios);
    }

    [HttpPost]
    [Limit(5)]
    public async Task<IActionResult> Create([FromBody] List<ItemPrecioDto> itemPreciosDto)
    {
        if (itemPreciosDto == null || itemPreciosDto.Count == 0) return BadRequest("ItemPrecios list cannot be null or empty");

        var firstRegistroId = itemPreciosDto.First().Registroid;

        if (itemPreciosDto.Any(item => item.Registroid != firstRegistroId)) return BadRequest("All items must have the same registroId");

        var userId = GetUserId();

        var registro = await _registrosLogic.GetRegistroAsync(firstRegistroId);
        
        if (registro == null) return BadRequest("Registro not found.");
        
        if (registro.Userid != userId) return Unauthorized("Unauthorized registro.");

        var success = await _itemPrecioLogic.CreateItemPreciosAsync(itemPreciosDto);

        return success ? Ok("ItemPrecios created successfully.") : BadRequest("Failed to create itemPrecios.");
    }

    [HttpDelete("{registroId}")]
    [Limit(5)]
    public async Task<IActionResult> Delete(long registroId)
    {
        if (registroId < 1) return BadRequest("Invalid registroId.");

        var registro = await _registrosLogic.GetRegistroAsync(registroId);

        var userId = GetUserId();

        if (registro == null) return BadRequest("Registro not found.");

        if (registro.Userid != userId) return Unauthorized("Unauthorized registro.");

        var success = await _itemPrecioLogic.DeleteItemPreciosAsync(registroId);

        return success ? Ok("ItemPrecios deleted successfully.") : BadRequest("Failed to delete itemPrecios.");
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}