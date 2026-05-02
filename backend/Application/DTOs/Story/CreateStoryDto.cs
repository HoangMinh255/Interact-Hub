using System.ComponentModel.DataAnnotations;

namespace InteractHub.Application.DTOs.Story;

public sealed class CreateStoryDto
{
    [MaxLength(2000)]
    public string? Content { get; set; }

    [MaxLength(2048)]
    public string? MediaUrl { get; set; }

    [MaxLength(100)]
    public string? MediaType { get; set; }

    /// <summary>
    /// Optional expiration time. If null, service will default to 24 hours from now (UTC).
    /// </summary>
    public DateTime? ExpireAt { get; set; }
}
