using System.Security.Claims;
using ApexGarage.Auth;
using ApexGarage.DTOs.Vehicles;
using ApexGarage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexGarage.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PolicyNames.CustomerOrAdmin)]
[Tags("Vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// Get all vehicles. Admins see all; customers see only their own.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        var vehicles = await _vehicleService.GetAllAsync(claimCustomerId, role);
        return Ok(vehicles);
    }

    /// <summary>
    /// Get a vehicle by ID. Customers can only access their own vehicles.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        var vehicle = await _vehicleService.GetByIdAsync(id, claimCustomerId, role);
        if (vehicle is null) return NotFound();
        return Ok(vehicle);
    }

    /// <summary>
    /// Register a new vehicle. Customers can only add to their own account.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] VehicleRequest request)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        var vehicle = await _vehicleService.CreateAsync(request, claimCustomerId, role);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// Update a vehicle. Customers can only update their own vehicles.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] VehicleRequest request)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        var vehicle = await _vehicleService.UpdateAsync(id, request, claimCustomerId, role);
        return Ok(vehicle);
    }

    /// <summary>
    /// Delete a vehicle. Customers can only delete their own vehicles.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        await _vehicleService.DeleteAsync(id, claimCustomerId, role);
        return NoContent();
    }
}
