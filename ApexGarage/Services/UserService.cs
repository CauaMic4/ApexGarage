using ApexGarage.Auth;
using ApexGarage.DTOs.Auth;
using ApexGarage.Entities;
using ApexGarage.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace ApexGarage.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;

    public UserService(
        IUserRepository userRepository,
        ICustomerRepository customerRepository,
        JwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator)
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        return await RegisterWithRoleAsync(request, Roles.Customer);
    }

    public async Task<AuthResponse> RegisterAdminAsync(RegisterRequest request)
    {
        return await RegisterWithRoleAsync(request, Roles.Admin);
    }

    private async Task<AuthResponse> RegisterWithRoleAsync(RegisterRequest request, string role)
    {
        var validation = await _registerValidator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        // Create Customer profile
        var customer = new Customer
        {
            FullName = request.FullName,
            Email = request.Email.ToLowerInvariant(),
            Phone = request.Phone,
            Address = request.Address
        };
        await _customerRepository.CreateAsync(customer);

        // Create User (auth identity)
        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role,
            CustomerId = customer.Id
        };
        await _userRepository.CreateAsync(user);

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            CustomerId = user.CustomerId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var validation = await _loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ArgumentException(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            CustomerId = user.CustomerId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
        };
    }
}
