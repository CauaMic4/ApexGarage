using ApexGarage.Configurations;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using Microsoft.Extensions.Options;

namespace ApexGarage.Repositories;

public class MechanicRepository : MongoRepository<Mechanic>, IMechanicRepository
{
    public MechanicRepository(IOptions<MongoDbSettings> settings)
        : base(settings, "mechanics") { }
}
