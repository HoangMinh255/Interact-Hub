using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.DTOs.Notification;
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
    public async Task<IList<Notification>> Get10NotificationsByRecipientId(string RecipientId, int page)
    {
        return await _notificationRepository.Get10NotificationsByRecipientId(RecipientId, page);
    }
    public async Task<Notification> CreateNotification(CreateNotificationDto notification)
    {
        var notificationEntity = new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = notification.RecipientId,
            ActorId = notification.ActorId,
            Type =(byte) notification.Type,
            Content = notification.Content,
            RelatedEntityType = notification.RelatedEntityType,
            RelatedEntityId = notification.RelatedEntityId,
            IsRead = false,
            CreatedAt = notification.CreatedAt,
            IsDeleted = false
        };
        return await _notificationRepository.CreateNotification(notificationEntity);
    }
    public async Task<bool> UpdateNotification(Guid id)
    {
        return await _notificationRepository.UpdateNotification(id);
    }
    public async Task<bool> DeleteNotification(Guid id)
    {
        return await _notificationRepository.DeleteNotification(id);
    }
    public async Task<IList<Notification>> GetNotificationsByUserId(string RecipientId)
    {
        return await _notificationRepository.GetNotificationsByUserId(RecipientId);
    }
}