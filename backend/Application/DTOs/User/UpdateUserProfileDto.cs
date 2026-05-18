using System.ComponentModel.DataAnnotations;

namespace InteractHub.Application.DTOs.User;

public sealed class UpdateUserProfileDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Bio { get; set; }

    public DateTime? DateOfBirth { get; set; }
}
