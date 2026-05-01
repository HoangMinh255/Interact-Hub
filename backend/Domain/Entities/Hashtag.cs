namespace InteractHub.Domain.Entities;

public class Hashtag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
}