namespace InteractHub.Application.DTOs.Comment;
public class CreateCommentDto
{
    public Guid PostId { get; set; }
    public string? UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = null!;
}