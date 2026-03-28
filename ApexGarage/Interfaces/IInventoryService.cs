using ApexGarage.DTOs.Inventory;

namespace ApexGarage.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<InventoryItemResponse>> GetAllAsync();
    Task<InventoryItemResponse?> GetByIdAsync(string id);
    Task<InventoryItemResponse> CreateAsync(InventoryItemRequest request);
    Task<InventoryItemResponse> UpdateAsync(string id, InventoryItemRequest request);
    Task DeleteAsync(string id);
}
