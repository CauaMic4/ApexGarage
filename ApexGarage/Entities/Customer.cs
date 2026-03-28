using MongoDB.Bson.Serialization.Attributes;

namespace ApexGarage.Entities;

[BsonIgnoreExtraElements]
public class Customer : BaseEntity
{
    [BsonElement("fullName")]
    public string FullName { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("phone")]
    public string Phone { get; set; } = null!;

    [BsonElement("address")]
    public string? Address { get; set; }
}
