using InteractHub.Domain.Base;
namespace InteractHub.Domain.Entities;

public class Notification : BaseEntity
{
    public string RecipientId { get; set; } = null!;
    public string? ActorId { get; set; }
    public byte Type { get; set; }
    public string Content { get; set; } = null!;
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityId { get; set; }
    public bool IsRead { get; set; } = false;

    // Navigation
    public ApplicationUser Recipient { get; set; } = null!;
    public ApplicationUser? Actor { get; set; }
}