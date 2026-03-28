using ApexGarage.Entities;

namespace ApexGarage.Interfaces;

public interface IInventoryRepository : IRepository<InventoryItem>
{
    Task<InventoryItem?> GetByPartNumberAsync(string partNumber);
}
