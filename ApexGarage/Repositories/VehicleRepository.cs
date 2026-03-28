using ApexGarage.Configurations;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApexGarage.Repositories;

public class VehicleRepository : MongoRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(IOptions<MongoDbSettings> settings)
        : base(settings, "vehicles") { }

    public async Task<IEnumerable<Vehicle>> GetByCustomerIdAsync(string customerId)
    {
        return await _collection.Find(v => v.CustomerId == customerId).ToListAsync();
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        var normalized = licensePlate.ToUpperInvariant();
        return await _collection.Find(v => v.LicensePlate == normalized).FirstOrDefaultAsync();
    }
}
