namespace ApexGarage.DTOs.Vehicles;

public class VehicleRequest
{
    public string CustomerId { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = null!;
    public int Mileage { get; set; }
    public string? AssignedMechanicId { get; set; }
}
