namespace InteractHub.Application.DTOs.Story;

public sealed class StoryFeedQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
