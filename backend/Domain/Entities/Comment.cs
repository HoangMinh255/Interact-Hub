using InteractHub.Domain.Base;
namespace InteractHub.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid PostId { get; set; }
    public string UserId { get; set; } = null!;
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = null!;
    // Navigation
    public Post Post { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}