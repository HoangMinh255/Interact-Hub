using System.ComponentModel.DataAnnotations;

namespace InteractHub.Application.DTOs.Story;

public sealed class UpdateStoryDto
{
    [MaxLength(2000)]
    public string? Content { get; set; }

    [MaxLength(2048)]
    public string? MediaUrl { get; set; }

    [MaxLength(100)]
    public string? MediaType { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? ExpireAt { get; set; }
}
