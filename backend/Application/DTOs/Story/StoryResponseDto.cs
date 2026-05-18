namespace InteractHub.Application.DTOs.Story;

public sealed class StoryResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? MediaUrl { get; set; }
    public byte MediaType { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
