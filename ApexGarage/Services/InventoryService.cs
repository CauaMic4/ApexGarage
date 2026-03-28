using ApexGarage.DTOs.Inventory;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using FluentValidation;

namespace ApexGarage.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IValidator<InventoryItemRequest> _validator;

    public InventoryService(IInventoryRepository inventoryRepository, IValidator<InventoryItemRequest> validator)
    {
        _inventoryRepository = inventoryRepository;
        _validator = validator;
    }

    public async Task<IEnumerable<InventoryItemResponse>> GetAllAsync()
    {
        var items = await _inventoryRepository.GetAllAsync();
        return items.Select(MapToResponse);
    }

    public async Task<InventoryItemResponse?> GetByIdAsync(string id)
    {
        var item = await _inventoryRepository.GetByIdAsync(id);
        return item is null ? null : MapToResponse(item);
    }

    public async Task<InventoryItemResponse> CreateAsync(InventoryItemRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var existing = await _inventoryRepository.GetByPartNumberAsync(request.PartNumber);
        if (existing is not null)
            throw new InvalidOperationException($"An item with part number '{request.PartNumber}' already exists.");

        var item = new InventoryItem
        {
            Name = request.Name,
            Description = request.Description,
            PartNumber = request.PartNumber,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            Category = request.Category
        };

        await _inventoryRepository.CreateAsync(item);
        return MapToResponse(item);
    }

    public async Task<InventoryItemResponse> UpdateAsync(string id, InventoryItemRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var item = await _inventoryRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Inventory item with ID '{id}' not found.");

        item.Name = request.Name;
        item.Description = request.Description;
        item.PartNumber = request.PartNumber;
        item.Quantity = request.Quantity;
        item.UnitPrice = request.UnitPrice;
        item.Category = request.Category;

        await _inventoryRepository.UpdateAsync(id, item);
        return MapToResponse(item);
    }

    public async Task DeleteAsync(string id)
    {
        var item = await _inventoryRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Inventory item with ID '{id}' not found.");

        await _inventoryRepository.DeleteAsync(id);
    }

    private static InventoryItemResponse MapToResponse(InventoryItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        PartNumber = item.PartNumber,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice,
        Category = item.Category,
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt
    };
}
