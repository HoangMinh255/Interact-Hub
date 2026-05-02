using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InteractHub.Application.DTOs.User;

public sealed class UploadAvatarRequestDto
{
    [Required]
    public IFormFile AvatarFile { get; set; } = default!;
}
