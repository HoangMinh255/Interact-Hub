namespace InteractHub.Application.DTOs.Friendship;
public class FriendDto
{
    public Guid FriendshipId { get; set; }
    public string FriendId { get; set; }     // ID của người bạn kia
    public string FriendName { get; set; }   // Tên của bạn
    public string FriendAvatar { get; set; } // Ảnh đại diện
}