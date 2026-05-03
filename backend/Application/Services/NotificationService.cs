using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.DTOs.Notification;
using InteractHub.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace InteractHub.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    public NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
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
        var createdNotification = await _notificationRepository.CreateNotification(notificationEntity);

        // Bắn thông báo Real-time bằng SignalR
        await _hubContext.Clients.User(notification.RecipientId)
            .SendAsync("ReceiveNewNotification", createdNotification);
        return createdNotification;
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