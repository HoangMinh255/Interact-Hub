using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
namespace InteractHub.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<IList<Notification>> GetAll();
    Task<Notification?> GetNotificationById(Guid id);
    Task<IList<Notification>> Get10NotificationsByRecipientId(string RecipientId, int page);
    Task<Notification> CreateNotification(Notification notification);
    Task<bool> UpdateNotification(Guid id);
    Task<bool> DeleteNotification(Guid id);
    Task<IList<Notification>> GetNotificationsByUserId(string RecipientId);
    Task SaveChanges();
}