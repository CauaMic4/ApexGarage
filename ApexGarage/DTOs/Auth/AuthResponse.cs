namespace ApexGarage.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? CustomerId { get; set; }
    public DateTime ExpiresAt { get; set; }
}
