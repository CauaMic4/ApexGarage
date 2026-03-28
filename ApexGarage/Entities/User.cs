using MongoDB.Bson.Serialization.Attributes;

namespace ApexGarage.Entities;

[BsonIgnoreExtraElements]
public class User : BaseEntity
{
    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;

    [BsonElement("role")]
    public string Role { get; set; } = null!;

    [BsonElement("customerId")]
    public string? CustomerId { get; set; }
}
