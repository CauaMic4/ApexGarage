using ApexGarage.Entities;

namespace ApexGarage.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<IEnumerable<Vehicle>> GetByCustomerIdAsync(string customerId);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
}
