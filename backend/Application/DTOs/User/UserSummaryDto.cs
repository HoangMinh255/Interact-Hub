namespace InteractHub.Application.DTOs.User;

public sealed class UserSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; }
}
