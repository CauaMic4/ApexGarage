using ApexGarage.Auth;
using ApexGarage.DTOs.Customers;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using FluentValidation;

namespace ApexGarage.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<CustomerRequest> _validator;

    public CustomerService(ICustomerRepository customerRepository, IValidator<CustomerRequest> validator)
    {
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToResponse);
    }

    public async Task<CustomerResponse?> GetByIdAsync(string id, string? claimCustomerId, string role)
    {
        // Customers can only see their own profile
        if (role == Roles.Customer && claimCustomerId != id)
            throw new UnauthorizedAccessException("You can only access your own profile.");

        var customer = await _customerRepository.GetByIdAsync(id);
        return customer is null ? null : MapToResponse(customer);
    }

    public async Task<CustomerResponse> CreateAsync(CustomerRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var existing = await _customerRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("A customer with this email already exists.");

        var customer = new Customer
        {
            FullName = request.FullName,
            Email = request.Email.ToLowerInvariant(),
            Phone = request.Phone,
            Address = request.Address
        };

        await _customerRepository.CreateAsync(customer);
        return MapToResponse(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(string id, CustomerRequest request, string? claimCustomerId, string role)
    {
        // Customers can only update their own profile
        if (role == Roles.Customer && claimCustomerId != id)
            throw new UnauthorizedAccessException("You can only update your own profile.");

        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var customer = await _customerRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Customer with ID '{id}' not found.");

        customer.FullName = request.FullName;
        customer.Email = request.Email.ToLowerInvariant();
        customer.Phone = request.Phone;
        customer.Address = request.Address;

        await _customerRepository.UpdateAsync(id, customer);
        return MapToResponse(customer);
    }

    public async Task DeleteAsync(string id)
    {
        var customer = await _customerRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Customer with ID '{id}' not found.");

        await _customerRepository.DeleteAsync(id);
    }

    private static CustomerResponse MapToResponse(Customer customer) => new()
    {
        Id = customer.Id,
        FullName = customer.FullName,
        Email = customer.Email,
        Phone = customer.Phone,
        Address = customer.Address,
        CreatedAt = customer.CreatedAt,
        UpdatedAt = customer.UpdatedAt
    };
}
