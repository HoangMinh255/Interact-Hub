using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
namespace InteractHub.Application.Interfaces.Services;
public interface INotificationService
{
    Task<IList<Notification>> GetAllNotifications();
    Task<Notification?> GetNotificationById(Guid id);
    Task<IActionResult> CreateNotification(Notification notification);
    Task<IActionResult> UpdateNotification(Notification notification);
    Task<IActionResult> DeleteNotification(Notification notification);
    Task<IList<Notification>> GetNotificationsByUserId(string RecipientId);
}