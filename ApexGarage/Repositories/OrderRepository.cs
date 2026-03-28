using ApexGarage.Configurations;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApexGarage.Repositories;

public class OrderRepository : MongoRepository<Order>, IOrderRepository
{
    public OrderRepository(IOptions<MongoDbSettings> settings)
        : base(settings, "orders") { }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId)
    {
        return await _collection.Find(o => o.CustomerId == customerId).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByMechanicIdAsync(string mechanicId)
    {
        return await _collection.Find(o => o.MechanicId == mechanicId).ToListAsync();
    }
}
