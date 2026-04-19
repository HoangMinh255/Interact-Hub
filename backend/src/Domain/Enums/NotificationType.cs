namespace InteractHub.Domain.Enums;

public enum NotificationType : byte
{
    FriendRequestReceived = 0,
    FriendRequestAccepted = 1,
    PostLiked = 2,
    PostCommented = 3,
    StoryViewed = 4,
    PostReported = 5,
    ReportReviewed = 6
}