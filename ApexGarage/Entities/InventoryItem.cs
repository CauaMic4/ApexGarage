using MongoDB.Bson.Serialization.Attributes;

namespace ApexGarage.Entities;

[BsonIgnoreExtraElements]
public class InventoryItem : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("partNumber")]
    public string PartNumber { get; set; } = null!;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unitPrice")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
    public decimal UnitPrice { get; set; }

    [BsonElement("category")]
    public string Category { get; set; } = null!;
}
