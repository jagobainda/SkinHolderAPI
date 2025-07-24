using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.Log;
using SkinHolderAPI.Attributes;
using SkinHolderAPI.DTOs.Log;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class LogController(ILogLogic logLogic) : ControllerBase
{
    private readonly ILogLogic _logLogic = logLogic;
    [HttpPost("AddLog")]
    [Limit(5)]
    public async Task<IActionResult> AddLog([FromBody] LoggerDto loggerDto)
    {
        if (loggerDto == null) return BadRequest("Log cannot be null.");

        var success = await _logLogic.AddLogAsync(loggerDto);

        return Ok(success);
    }
}
