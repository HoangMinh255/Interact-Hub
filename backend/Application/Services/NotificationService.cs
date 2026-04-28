using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
namespace InteractHub.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    public async Task<IList<Notification>> GetAllNotifications()
    {
        return await _notificationRepository.GetAll();
    }
    public async Task<Notification?> GetNotificationById(Guid id)
    {
        return await _notificationRepository.GetNotificationById(id);
    }
    public async Task<IActionResult> CreateNotification(Notification notification)
    {
        return await _notificationRepository.CreateNotification(notification);
    }
    public async Task<IActionResult> UpdateNotification(Notification notification)
    {
        return await _notificationRepository.UpdateNotification(notification);
    }
    public async Task<IActionResult> DeleteNotification(Notification notification)
    {
        return await _notificationRepository.DeleteNotification(notification);
    }
    public async Task<IList<Notification>> GetNotificationsByUserId(string RecipientId)
    {
        return await _notificationRepository.GetNotificationsByUserId(RecipientId);
    }
}