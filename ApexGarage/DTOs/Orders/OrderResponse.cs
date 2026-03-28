namespace ApexGarage.DTOs.Orders;

public class OrderItemResponse
{
    public string InventoryItemId { get; set; } = null!;
    public string ItemName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

public class OrderResponse
{
    public string Id { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public string VehicleId { get; set; } = null!;
    public string MechanicId { get; set; } = null!;
    public List<OrderItemResponse> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
