using ApexGarage.DTOs.Mechanics;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using FluentValidation;

namespace ApexGarage.Services;

public class MechanicService : IMechanicService
{
    private readonly IMechanicRepository _mechanicRepository;
    private readonly IValidator<MechanicRequest> _validator;

    public MechanicService(IMechanicRepository mechanicRepository, IValidator<MechanicRequest> validator)
    {
        _mechanicRepository = mechanicRepository;
        _validator = validator;
    }

    public async Task<IEnumerable<MechanicResponse>> GetAllAsync()
    {
        var mechanics = await _mechanicRepository.GetAllAsync();
        return mechanics.Select(MapToResponse);
    }

    public async Task<MechanicResponse?> GetByIdAsync(string id)
    {
        var mechanic = await _mechanicRepository.GetByIdAsync(id);
        return mechanic is null ? null : MapToResponse(mechanic);
    }

    public async Task<MechanicResponse> CreateAsync(MechanicRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var mechanic = new Mechanic
        {
            FullName = request.FullName,
            Specialty = request.Specialty,
            Phone = request.Phone,
            IsActive = request.IsActive
        };

        await _mechanicRepository.CreateAsync(mechanic);
        return MapToResponse(mechanic);
    }

    public async Task<MechanicResponse> UpdateAsync(string id, MechanicRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var mechanic = await _mechanicRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Mechanic with ID '{id}' not found.");

        mechanic.FullName = request.FullName;
        mechanic.Specialty = request.Specialty;
        mechanic.Phone = request.Phone;
        mechanic.IsActive = request.IsActive;

        await _mechanicRepository.UpdateAsync(id, mechanic);
        return MapToResponse(mechanic);
    }

    public async Task DeleteAsync(string id)
    {
        var mechanic = await _mechanicRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Mechanic with ID '{id}' not found.");

        await _mechanicRepository.DeleteAsync(id);
    }

    private static MechanicResponse MapToResponse(Mechanic mechanic) => new()
    {
        Id = mechanic.Id,
        FullName = mechanic.FullName,
        Specialty = mechanic.Specialty,
        Phone = mechanic.Phone,
        IsActive = mechanic.IsActive,
        CreatedAt = mechanic.CreatedAt,
        UpdatedAt = mechanic.UpdatedAt
    };
}
