using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}