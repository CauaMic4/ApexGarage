using ApexGarage.Entities;

namespace ApexGarage.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
