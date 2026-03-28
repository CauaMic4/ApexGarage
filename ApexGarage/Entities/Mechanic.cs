using MongoDB.Bson.Serialization.Attributes;

namespace ApexGarage.Entities;

[BsonIgnoreExtraElements]
public class Mechanic : BaseEntity
{
    [BsonElement("fullName")]
    public string FullName { get; set; } = null!;

    [BsonElement("specialty")]
    public string Specialty { get; set; } = null!;

    [BsonElement("phone")]
    public string Phone { get; set; } = null!;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}
