using MongoDB.Bson.Serialization.Attributes;

namespace ApexGarage.Entities;

[BsonIgnoreExtraElements]
public class Vehicle : BaseEntity
{
    [BsonElement("customerId")]
    public string CustomerId { get; set; } = null!;

    [BsonElement("brand")]
    public string Brand { get; set; } = null!;

    [BsonElement("model")]
    public string Model { get; set; } = null!;

    [BsonElement("year")]
    public int Year { get; set; }

    [BsonElement("licensePlate")]
    public string LicensePlate { get; set; } = null!;

    [BsonElement("mileage")]
    public int Mileage { get; set; }

    [BsonElement("assignedMechanicId")]
    public string? AssignedMechanicId { get; set; }
}
