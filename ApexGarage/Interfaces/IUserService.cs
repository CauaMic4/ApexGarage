using ApexGarage.DTOs.Auth;

namespace ApexGarage.Interfaces;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAdminAsync(RegisterRequest request);
}
