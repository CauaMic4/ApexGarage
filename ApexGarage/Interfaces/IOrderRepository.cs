using ApexGarage.Entities;

namespace ApexGarage.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<Order>> GetByMechanicIdAsync(string mechanicId);
}
