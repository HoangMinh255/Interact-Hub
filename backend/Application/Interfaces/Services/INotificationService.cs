using InteractHub.Domain.Entities;
using InteractHub.Application.DTOs.Notification;
using Microsoft.AspNetCore.Mvc;
namespace InteractHub.Application.Interfaces.Services;
public interface INotificationService
{
    Task<IList<Notification>> GetAllNotifications();
    Task<Notification?> GetNotificationById(Guid id);
    Task<IList<Notification>> Get10NotificationsByRecipientId(string RecipientId, int page);
    Task<Notification> CreateNotification(CreateNotificationDto notification);
    Task<bool> UpdateNotification(Guid id);
    Task<bool> DeleteNotification(Guid id);
    Task<IList<Notification>> GetNotificationsByUserId(string RecipientId);
}