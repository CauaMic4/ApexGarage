using System.Security.Claims;
using ApexGarage.Auth;
using ApexGarage.DTOs.Customers;
using ApexGarage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexGarage.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PolicyNames.CustomerOrAdmin)]
[Tags("Customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Get all customers. Admin only.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    /// <summary>
    /// Get a customer by ID. Customers can only access their own profile.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        var customer = await _customerService.GetByIdAsync(id, claimCustomerId, role);
        if (customer is null) return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Create a new customer. Admin only.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CustomerRequest request)
    {
        var customer = await _customerService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    /// <summary>
    /// Update a customer. Customers can only update their own profile.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] CustomerRequest request)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var claimCustomerId = User.FindFirstValue("customerId");

        var customer = await _customerService.UpdateAsync(id, request, claimCustomerId, role);
        return Ok(customer);
    }

    /// <summary>
    /// Delete a customer. Admin only.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await _customerService.DeleteAsync(id);
        return NoContent();
    }
}
