using ApexGarage.DTOs.Orders;

namespace ApexGarage.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderResponse>> GetAllAsync(string? claimCustomerId, string role);
    Task<OrderResponse?> GetByIdAsync(string id, string? claimCustomerId, string role);
    Task<OrderResponse> CreateAsync(OrderRequest request);
    Task<OrderResponse> UpdateStatusAsync(string id, string status);
    Task DeleteAsync(string id);
}
