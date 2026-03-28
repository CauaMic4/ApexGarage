using ApexGarage.Auth;
using ApexGarage.DTOs.Vehicles;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using FluentValidation;

namespace ApexGarage.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<VehicleRequest> _validator;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        ICustomerRepository customerRepository,
        IValidator<VehicleRequest> validator)
    {
        _vehicleRepository = vehicleRepository;
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task<IEnumerable<VehicleResponse>> GetAllAsync(string? claimCustomerId, string role)
    {
        IEnumerable<Vehicle> vehicles;

        if (role == Roles.Customer && !string.IsNullOrEmpty(claimCustomerId))
            vehicles = await _vehicleRepository.GetByCustomerIdAsync(claimCustomerId);
        else
            vehicles = await _vehicleRepository.GetAllAsync();

        return vehicles.Select(MapToResponse);
    }

    public async Task<VehicleResponse?> GetByIdAsync(string id, string? claimCustomerId, string role)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle is null) return null;

        if (role == Roles.Customer && vehicle.CustomerId != claimCustomerId)
            throw new UnauthorizedAccessException("You can only access your own vehicles.");

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> CreateAsync(VehicleRequest request, string? claimCustomerId, string role)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        // Customers can only add vehicles for themselves
        if (role == Roles.Customer && request.CustomerId != claimCustomerId)
            throw new UnauthorizedAccessException("You can only add vehicles to your own account.");

        // Verify customer exists
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId)
            ?? throw new KeyNotFoundException($"Customer with ID '{request.CustomerId}' not found.");

        // Check license plate uniqueness
        var existingPlate = await _vehicleRepository.GetByLicensePlateAsync(request.LicensePlate);
        if (existingPlate is not null)
            throw new InvalidOperationException($"A vehicle with license plate '{request.LicensePlate}' already exists.");

        var vehicle = new Vehicle
        {
            CustomerId = request.CustomerId,
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            LicensePlate = request.LicensePlate.ToUpperInvariant(),
            Mileage = request.Mileage,
            AssignedMechanicId = request.AssignedMechanicId
        };

        await _vehicleRepository.CreateAsync(vehicle);
        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> UpdateAsync(string id, VehicleRequest request, string? claimCustomerId, string role)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var vehicle = await _vehicleRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Vehicle with ID '{id}' not found.");

        if (role == Roles.Customer && vehicle.CustomerId != claimCustomerId)
            throw new UnauthorizedAccessException("You can only update your own vehicles.");

        vehicle.Brand = request.Brand;
        vehicle.Model = request.Model;
        vehicle.Year = request.Year;
        vehicle.LicensePlate = request.LicensePlate.ToUpperInvariant();
        vehicle.Mileage = request.Mileage;
        vehicle.AssignedMechanicId = request.AssignedMechanicId;

        await _vehicleRepository.UpdateAsync(id, vehicle);
        return MapToResponse(vehicle);
    }

    public async Task DeleteAsync(string id, string? claimCustomerId, string role)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Vehicle with ID '{id}' not found.");

        if (role == Roles.Customer && vehicle.CustomerId != claimCustomerId)
            throw new UnauthorizedAccessException("You can only delete your own vehicles.");

        await _vehicleRepository.DeleteAsync(id);
    }

    private static VehicleResponse MapToResponse(Vehicle vehicle) => new()
    {
        Id = vehicle.Id,
        CustomerId = vehicle.CustomerId,
        Brand = vehicle.Brand,
        Model = vehicle.Model,
        Year = vehicle.Year,
        LicensePlate = vehicle.LicensePlate,
        Mileage = vehicle.Mileage,
        AssignedMechanicId = vehicle.AssignedMechanicId,
        CreatedAt = vehicle.CreatedAt,
        UpdatedAt = vehicle.UpdatedAt
    };
}
