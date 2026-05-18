namespace InteractHub.Infrastructure.Options;

public class IdentitySeedOptions
{
    public const string SectionName = "SeedAdmin";

    public string Email { get; set; } = "admin@interacthub.local";
    public string Password { get; set; } = "Admin@12345";
    public string FullName { get; set; } = "System Admin";
}