using ApexGarage.Configurations;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApexGarage.Repositories;

public class CustomerRepository : MongoRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(IOptions<MongoDbSettings> settings)
        : base(settings, "customers") { }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _collection.Find(c => c.Email == normalizedEmail).FirstOrDefaultAsync();
    }
}
