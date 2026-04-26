using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}