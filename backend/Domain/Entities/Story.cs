namespace InteractHub.Domain.Entities;
using InteractHub.Domain.Base;

public class Story : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string? Content { get; set; }
    public string MediaUrl { get; set; } = null!;
    public byte MediaType { get; set; } // 0=Image, 1=Video
    public DateTime ExpireAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}