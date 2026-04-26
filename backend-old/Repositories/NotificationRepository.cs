using Microsoft.EntityFrameworkCore;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Notifications>> GetAll()
    {
        return await _context.Notifications.ToListAsync();
    }
    public async Task<Notifications?> GetNotificationById(int id)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task CreateNotification(Notifications notification)
    {
        await _context.Notifications.AddAsync(notification);
        await SaveChanges();
    }

    public async Task UpdateNotification(Notifications notification)
    {
        _context.Notifications.Update(notification);
        await SaveChanges();
    }

    public async Task DeleteNotification(Notifications notification)
    {
        _context.Notifications.Remove(notification);
        await SaveChanges();
    }

    public async Task<IList<Notifications>> GetNotificationsByUserId(string userId)
    {
        return await _context.Notifications.Where(n => n.UserId.ToString() == userId).ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}