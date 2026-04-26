public interface INotificationRepository
{
    Task<IList<Notifications>> GetAll();
    Task<Notifications?> GetNotificationById(int id);
    Task CreateNotification(Notifications notification);
    Task UpdateNotification(Notifications notification);
    Task DeleteNotification(Notifications notification);
    Task<IList<Notifications>> GetNotificationsByUserId(string userId);
    Task SaveChanges();
}