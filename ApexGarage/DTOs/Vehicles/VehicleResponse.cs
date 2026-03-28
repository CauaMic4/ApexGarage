namespace ApexGarage.DTOs.Vehicles;

public class VehicleResponse
{
    public string Id { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = null!;
    public int Mileage { get; set; }
    public string? AssignedMechanicId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
