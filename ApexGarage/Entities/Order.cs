using MongoDB.Bson.Serialization.Attributes;

namespace ApexGarage.Entities;

[BsonIgnoreExtraElements]
public class Order : BaseEntity
{
    [BsonElement("customerId")]
    public string CustomerId { get; set; } = null!;

    [BsonElement("vehicleId")]
    public string VehicleId { get; set; } = null!;

    [BsonElement("mechanicId")]
    public string MechanicId { get; set; } = null!;

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();

    [BsonElement("totalAmount")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
    public decimal TotalAmount { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = OrderStatuses.Pending;

    [BsonElement("notes")]
    public string? Notes { get; set; }
}

public static class OrderStatuses
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
}
