using ApexGarage.Configurations;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApexGarage.Repositories;

public class InventoryRepository : MongoRepository<InventoryItem>, IInventoryRepository
{
    public InventoryRepository(IOptions<MongoDbSettings> settings)
        : base(settings, "inventory") { }

    public async Task<InventoryItem?> GetByPartNumberAsync(string partNumber)
    {
        return await _collection.Find(i => i.PartNumber == partNumber).FirstOrDefaultAsync();
    }
}
