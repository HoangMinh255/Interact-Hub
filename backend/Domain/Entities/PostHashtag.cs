namespace InteractHub.Domain.Entities;

public class PostHashtag
{
    public Guid PostId { get; set; }
    public Guid HashtagId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Post Post { get; set; } = null!;
    public Hashtag Hashtag { get; set; } = null!;
}