using InteractHub.Domain.Entities;

namespace InteractHub.Application.DTOs.Post;
public class UpdatePostDto
{
    public string Content { get; set; } = null!;
    public int Visibility { get; set; } = 0; // 0=Public, 1=Friends, 2=OnlyMe
    public List<MediaItemDto> Media { get; set; } = new List<MediaItemDto>();
    public List<string>? Hashtags { get; set; }
}