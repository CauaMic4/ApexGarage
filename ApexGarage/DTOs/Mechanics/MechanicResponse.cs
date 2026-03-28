namespace ApexGarage.DTOs.Mechanics;

public class MechanicResponse
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Specialty { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
