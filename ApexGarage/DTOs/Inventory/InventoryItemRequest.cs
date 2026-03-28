namespace ApexGarage.DTOs.Inventory;

public class InventoryItemRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string PartNumber { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Category { get; set; } = null!;
}
