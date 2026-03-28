using ApexGarage.Auth;
using ApexGarage.DTOs.Orders;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using FluentValidation;

namespace ApexGarage.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMechanicRepository _mechanicRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IValidator<OrderRequest> _validator;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IVehicleRepository vehicleRepository,
        IMechanicRepository mechanicRepository,
        IInventoryRepository inventoryRepository,
        IValidator<OrderRequest> validator)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
        _mechanicRepository = mechanicRepository;
        _inventoryRepository = inventoryRepository;
        _validator = validator;
    }

    public async Task<IEnumerable<OrderResponse>> GetAllAsync(string? claimCustomerId, string role)
    {
        IEnumerable<Order> orders;

        if (role == Roles.Customer && !string.IsNullOrEmpty(claimCustomerId))
            orders = await _orderRepository.GetByCustomerIdAsync(claimCustomerId);
        else
            orders = await _orderRepository.GetAllAsync();

        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse?> GetByIdAsync(string id, string? claimCustomerId, string role)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null) return null;

        if (role == Roles.Customer && order.CustomerId != claimCustomerId)
            throw new UnauthorizedAccessException("You can only access your own orders.");

        return MapToResponse(order);
    }

    public async Task<OrderResponse> CreateAsync(OrderRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        // Validate referenced entities exist
        _ = await _customerRepository.GetByIdAsync(request.CustomerId)
            ?? throw new KeyNotFoundException($"Customer with ID '{request.CustomerId}' not found.");

        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId)
            ?? throw new KeyNotFoundException($"Vehicle with ID '{request.VehicleId}' not found.");

        if (vehicle.CustomerId != request.CustomerId)
            throw new ArgumentException("The specified vehicle does not belong to the specified customer.");

        _ = await _mechanicRepository.GetByIdAsync(request.MechanicId)
            ?? throw new KeyNotFoundException($"Mechanic with ID '{request.MechanicId}' not found.");

        // Resolve order items from inventory — denormalize name/price and deduct stock
        var orderItems = new List<OrderItem>();
        foreach (var itemReq in request.Items)
        {
            var inventoryItem = await _inventoryRepository.GetByIdAsync(itemReq.InventoryItemId)
                ?? throw new KeyNotFoundException($"Inventory item with ID '{itemReq.InventoryItemId}' not found.");

            if (inventoryItem.Quantity < itemReq.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{inventoryItem.Name}'. Available: {inventoryItem.Quantity}, Requested: {itemReq.Quantity}.");

            // Deduct stock
            inventoryItem.Quantity -= itemReq.Quantity;
            await _inventoryRepository.UpdateAsync(inventoryItem.Id, inventoryItem);

            orderItems.Add(new OrderItem
            {
                InventoryItemId = inventoryItem.Id,
                ItemName = inventoryItem.Name,
                Quantity = itemReq.Quantity,
                UnitPrice = inventoryItem.UnitPrice
            });
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            VehicleId = request.VehicleId,
            MechanicId = request.MechanicId,
            Items = orderItems,
            TotalAmount = orderItems.Sum(i => i.Quantity * i.UnitPrice),
            Status = OrderStatuses.Pending,
            Notes = request.Notes
        };

        await _orderRepository.CreateAsync(order);
        return MapToResponse(order);
    }

    public async Task<OrderResponse> UpdateStatusAsync(string id, string status)
    {
        var validStatuses = new[] { OrderStatuses.Pending, OrderStatuses.InProgress, OrderStatuses.Completed, OrderStatuses.Cancelled };
        if (!validStatuses.Contains(status))
            throw new ArgumentException($"Invalid status '{status}'. Valid values: {string.Join(", ", validStatuses)}.");

        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Order with ID '{id}' not found.");

        order.Status = status;
        await _orderRepository.UpdateAsync(id, order);
        return MapToResponse(order);
    }

    public async Task DeleteAsync(string id)
    {
        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Order with ID '{id}' not found.");

        await _orderRepository.DeleteAsync(id);
    }

    private static OrderResponse MapToResponse(Order order) => new()
    {
        Id = order.Id,
        CustomerId = order.CustomerId,
        VehicleId = order.VehicleId,
        MechanicId = order.MechanicId,
        Items = order.Items.Select(i => new OrderItemResponse
        {
            InventoryItemId = i.InventoryItemId,
            ItemName = i.ItemName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList(),
        TotalAmount = order.TotalAmount,
        Status = order.Status,
        Notes = order.Notes,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };
}
