namespace InteractHub.Domain.Entities;

public class Like
{
    public Guid PostId { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Post Post { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}