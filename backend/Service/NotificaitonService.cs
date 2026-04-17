using Microsoft.CodeAnalysis.Elfie.Serialization;

public class NotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }
    public async Task<IList<Notifications>> GetAllNotifications()
    {
        return await _notificationRepository.GetAll();
    }
    public async Task<Notifications?> GetNotificationById(int id)
    {
        return await _notificationRepository.GetNotificationById(id);
    }
    public async Task CreateNotification(Notifications notification)
    {
        await _notificationRepository.CreateNotification(notification);
    }
    public async Task UpdateNotification(Notifications notification)
    {
        await _notificationRepository.UpdateNotification(notification);
    }
    public async Task DeleteNotification(Notifications notification)
    {
        await _notificationRepository.DeleteNotification(notification);
    }
    public async Task<IList<Notifications>> GetNotificationsByUserId(string userId)
    {
        return await _notificationRepository.GetNotificationsByUserId(userId);
    }
}