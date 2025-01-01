using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UperController : ControllerBase
{
    private readonly IRepository _repository;

    public UperController(IRepository repository)
    {
        _repository = repository;
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found in token.");
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateDto dto)
    {
        var userId = GetUserId();
        await _repository.CreateAsync(dto);
        return Ok("Data created successfully.");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] CreateUpdateDto dto)
    {
        var userId = GetUserId();
        await _repository.UpdateAsync(dto);
        return Ok("Data updated successfully.");
    }

    [HttpGet("all/{type}")]
    public async Task<IActionResult> GetAllAsync(string type)
    {
        var userId = GetUserId();
        var data = await _repository.GetAllAsync(type, userId);
        return Ok(data);
    }

    [HttpGet("{type}/{id}")]
    public async Task<IActionResult> GetByIdAsync(string type, string id)
    {
        var userId = GetUserId();
        var data = await _repository.GetByIdAsync(type, id, userId);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    [HttpDelete("{type}/{id}")]
    public async Task<IActionResult> DeleteAsync(string type, string id)
    {
        var userId = GetUserId();
        await _repository.DeleteAsync(type, id, userId);
        return Ok("Data deleted successfully.");
    }
}
