using System.Text;
using ApexGarage.Auth;
using ApexGarage.Configurations;
using ApexGarage.Interfaces;
using ApexGarage.Repositories;
using ApexGarage.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ApexGarage.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        // Repositories
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IVehicleRepository, VehicleRepository>();
        services.AddSingleton<IInventoryRepository, InventoryRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IMechanicRepository, MechanicRepository>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMechanicService, MechanicService>();

        // JWT
        var jwtSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSection);
        services.AddSingleton<JwtTokenService>();

        var jwtSettings = jwtSection.Get<JwtSettings>()!;
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Authorization Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.AdminOnly, policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(PolicyNames.CustomerOrAdmin, policy => policy.RequireRole(Roles.Admin, Roles.Customer));
        });

        // FluentValidation — auto-registers all validators in the assembly
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}
