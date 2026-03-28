using ApexGarage.Configurations;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApexGarage.Repositories;

public class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(IOptions<MongoDbSettings> settings)
        : base(settings, "users") { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _collection.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
    }
}
