using ApexGarage.DTOs.Mechanics;

namespace ApexGarage.Interfaces;

public interface IMechanicService
{
    Task<IEnumerable<MechanicResponse>> GetAllAsync();
    Task<MechanicResponse?> GetByIdAsync(string id);
    Task<MechanicResponse> CreateAsync(MechanicRequest request);
    Task<MechanicResponse> UpdateAsync(string id, MechanicRequest request);
    Task DeleteAsync(string id);
}
