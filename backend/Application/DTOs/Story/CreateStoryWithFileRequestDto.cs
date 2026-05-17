using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InteractHub.Application.DTOs.Story;

public sealed class CreateStoryWithFileRequestDto
{
    [MaxLength(2000)]
    public string? Content { get; set; }

    public DateTime? ExpireAt { get; set; }

    [Required]
    public IFormFile MediaFile { get; set; } = default!;
}
