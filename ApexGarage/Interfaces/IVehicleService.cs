using ApexGarage.DTOs.Vehicles;

namespace ApexGarage.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<VehicleResponse>> GetAllAsync(string? claimCustomerId, string role);
    Task<VehicleResponse?> GetByIdAsync(string id, string? claimCustomerId, string role);
    Task<VehicleResponse> CreateAsync(VehicleRequest request, string? claimCustomerId, string role);
    Task<VehicleResponse> UpdateAsync(string id, VehicleRequest request, string? claimCustomerId, string role);
    Task DeleteAsync(string id, string? claimCustomerId, string role);
}
