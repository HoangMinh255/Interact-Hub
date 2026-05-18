using InteractHub.Domain.Base;

namespace InteractHub.Domain.Entities;

public class PostShare : BaseEntity
{
    public Guid PostId { get; set; }
    public string SharerId { get; set; } = null!;
    public string? Comment { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
}
