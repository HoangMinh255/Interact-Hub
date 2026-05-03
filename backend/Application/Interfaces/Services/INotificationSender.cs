using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Services;

public interface INotificationSender
{
    // Hàm này chỉ định nghĩa, không code logic ở đây
    Task SendNotificationAsync(string userId, Notification notification);
}