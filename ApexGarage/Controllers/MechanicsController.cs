using ApexGarage.Auth;
using ApexGarage.DTOs.Mechanics;
using ApexGarage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexGarage.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PolicyNames.AdminOnly)]
[Tags("Mechanics")]
public class MechanicsController : ControllerBase
{
    private readonly IMechanicService _mechanicService;

    public MechanicsController(IMechanicService mechanicService)
    {
        _mechanicService = mechanicService;
    }

    /// <summary>
    /// Get all mechanics. Admin only.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MechanicResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var mechanics = await _mechanicService.GetAllAsync();
        return Ok(mechanics);
    }

    /// <summary>
    /// Get a mechanic by ID. Admin only.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MechanicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var mechanic = await _mechanicService.GetByIdAsync(id);
        if (mechanic is null) return NotFound();
        return Ok(mechanic);
    }

    /// <summary>
    /// Create a new mechanic. Admin only.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MechanicResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] MechanicRequest request)
    {
        var mechanic = await _mechanicService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = mechanic.Id }, mechanic);
    }

    /// <summary>
    /// Update a mechanic. Admin only.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MechanicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] MechanicRequest request)
    {
        var mechanic = await _mechanicService.UpdateAsync(id, request);
        return Ok(mechanic);
    }

    /// <summary>
    /// Delete a mechanic. Admin only.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await _mechanicService.DeleteAsync(id);
        return NoContent();
    }
}
