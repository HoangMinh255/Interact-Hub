namespace InteractHub.Application.DTOs.User;

public sealed class UserProfileResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
