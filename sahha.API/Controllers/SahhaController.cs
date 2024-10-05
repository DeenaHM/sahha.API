using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sahha.API.Contracts;
using sahha.API.Serives;

namespace sahha.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SahhaController : ControllerBase
{
    private readonly ISahhaService _sahhaService;

    public SahhaController(ISahhaService sahhaService)
    {
        _sahhaService = sahhaService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterProfile([FromBody] RegisterProfileRequest request)
    {
        if (string.IsNullOrEmpty(request.ExternalId.ToString()))
        {
            return BadRequest("externalId is required");
        }

        try
        {
            var response = await _sahhaService.RegisterProfileAsync(request.ExternalId);
            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Error registering profile: {ex.Message}");
        }
    }

    [HttpGet("get-biomarkers")]
    public async Task<IActionResult> GetBiomarkers(
        [FromQuery] Guid externalId,
        [FromQuery] string? categories,
        [FromQuery] string? types,
        [FromQuery] DateTime startDateTime,
        [FromQuery] DateTime endDateTime)
    {
        //if (externalId == Guid.Empty || string.IsNullOrEmpty(categories) || string.IsNullOrEmpty(types))
        //{
        //    return BadRequest("All parameters (externalId, categories, types, startDateTime, endDateTime) are required.");
        //}

        try
        {
            var response = await _sahhaService.GetBiomarkersAsync(externalId, categories, types, startDateTime, endDateTime);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving biomarkers: {ex.Message}");
        }
    }
}
