namespace InteractHub.Application.DTOs.Friendship;

public class CreateFriendRequestDto
{
    public string RequesterId {get; set;}
    public string ReceiverId {get; set;}
    public DateTime RequestedAt {get; set;} = DateTime.UtcNow;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}