namespace InteractHub.Application.DTOs.Post;
public class MediaItemDto
{
    public string MediaUrl { get; set; }
    public int MediaType { get; set; } = 0; // 0=Image, 1=Video
}