using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
namespace InteractHub.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<IList<Notification>> GetAll();
    Task<Notification?> GetNotificationById(Guid id);
    Task<IActionResult> CreateNotification(Notification notification);
    Task<IActionResult> UpdateNotification(Notification notification);
    Task<IActionResult> DeleteNotification(Notification notification);
    Task<IList<Notification>> GetNotificationsByUserId(string RecipientId);
    Task SaveChanges();
}