using ApexGarage.DTOs.Customers;

namespace ApexGarage.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponse>> GetAllAsync();
    Task<CustomerResponse?> GetByIdAsync(string id, string? claimCustomerId, string role);
    Task<CustomerResponse> CreateAsync(CustomerRequest request);
    Task<CustomerResponse> UpdateAsync(string id, CustomerRequest request, string? claimCustomerId, string role);
    Task DeleteAsync(string id);
}
