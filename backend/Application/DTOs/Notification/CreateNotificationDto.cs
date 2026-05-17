
namespace InteractHub.Application.DTOs.Notification;
public class CreateNotificationDto
{
    public string RecipientId { get; set; } = null!;
    public string? ActorId { get; set; }
    public int Type { get; set; }
    public string Content { get; set; } = null!;
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityId { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}