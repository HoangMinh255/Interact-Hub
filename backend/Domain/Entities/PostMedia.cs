using InteractHub.Domain.Base;
namespace InteractHub.Domain.Entities;

public class PostMedia : BaseEntity
{
    public Guid PostId { get; set; }
    public string MediaUrl { get; set; } = null!;
    public byte MediaType { get; set; } = 0; // 0=Image, 1=Video
    public int SortOrder { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
}