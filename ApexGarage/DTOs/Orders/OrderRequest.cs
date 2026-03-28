namespace ApexGarage.DTOs.Orders;

public class OrderItemRequest
{
    public string InventoryItemId { get; set; } = null!;
    public int Quantity { get; set; }
}

public class OrderRequest
{
    public string CustomerId { get; set; } = null!;
    public string VehicleId { get; set; } = null!;
    public string MechanicId { get; set; } = null!;
    public List<OrderItemRequest> Items { get; set; } = new();
    public string? Notes { get; set; }
}
