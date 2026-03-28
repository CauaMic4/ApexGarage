using System.Security.Claims;
using ApexGarage.Auth;
using ApexGarage.DTOs.Orders;
using ApexGarage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexGarage.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PolicyNames.CustomerOrAdmin)]
[Tags("Orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get all orders. Admins see all; customers see only their own.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        var orders = await _orderService.GetAllAsync(claimCustomerId, role);
        return Ok(orders);
    }

    /// <summary>
    /// Get an order by ID. Customers can only access their own orders.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        var order = await _orderService.GetByIdAsync(id, claimCustomerId, role);
        if (order is null) return NotFound();
        return Ok(order);
    }

    /// <summary>
    /// Create a new order. Admin only.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] OrderRequest request)
    {
        var order = await _orderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Update order status (Pending, InProgress, Completed, Cancelled). Admin only.
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(string id, [FromQuery] string status)
    {
        var order = await _orderService.UpdateStatusAsync(id, status);
        return Ok(order);
    }

    /// <summary>
    /// Delete an order. Admin only.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await _orderService.DeleteAsync(id);
        return NoContent();
    }
}
