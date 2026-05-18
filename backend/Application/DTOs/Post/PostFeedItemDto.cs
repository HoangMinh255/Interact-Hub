namespace InteractHub.Application.DTOs.Post;

public class PostFeedItemDto
{
    public Guid Id { get; set; }
    public Guid OriginalPostId { get; set; }
    public bool IsShared { get; set; }
    public string? OriginalContent { get; set; }
    public string? OriginalAuthorId { get; set; }
    public string? OriginalAuthorName { get; set; }
    public string? OriginalAuthorAvatar { get; set; }
    public string AuthorId { get; set; } = null!;
    public string? AuthorName { get; set; }
    public string? AuthorAvatar { get; set; }
    public string Content { get; set; } = null!;
    public byte Visibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string>? MediaUrls { get; set; }
    public int CommentCount { get; set; }
    public List<string>? Hashtag { get; set; }
}
