namespace InteractHub.Application.DTOs.Comment;

public class UpdateCommentDto
{
    public string Content { get; set; } = null!;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}