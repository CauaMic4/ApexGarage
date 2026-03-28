using MongoDB.Bson.Serialization.Attributes;

namespace ApexGarage.Entities;

public class OrderItem
{
    [BsonElement("inventoryItemId")]
    public string InventoryItemId { get; set; } = null!;

    [BsonElement("itemName")]
    public string ItemName { get; set; } = null!;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unitPrice")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
    public decimal UnitPrice { get; set; }
}
