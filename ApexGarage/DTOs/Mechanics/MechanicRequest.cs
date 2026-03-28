namespace ApexGarage.DTOs.Mechanics;

public class MechanicRequest
{
    public string FullName { get; set; } = null!;
    public string Specialty { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
