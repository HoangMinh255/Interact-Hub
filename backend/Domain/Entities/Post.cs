using InteractHub.Domain.Base;
namespace InteractHub.Domain.Entities;

public class Post : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public byte Visibility { get; set; } = 0; // 0=Public, 1=Friends, 2=OnlyMe

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<PostReport> Reports { get; set; } = new List<PostReport>();
    public ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
}