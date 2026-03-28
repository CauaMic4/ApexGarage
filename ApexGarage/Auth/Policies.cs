namespace ApexGarage.Auth;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";
}

public static class PolicyNames
{
    public const string AdminOnly = "AdminOnly";
    public const string CustomerOrAdmin = "CustomerOrAdmin";
}
