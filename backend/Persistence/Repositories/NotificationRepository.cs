using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Persistence.Data;
using Microsoft.AspNetCore.Mvc;

namespace InteractHub.Persistence.Repositories;
public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Notification>> GetAll()
    {
        return await _context.Notifications.ToListAsync();
    }
    public async Task<Notification?> GetNotificationById(Guid id)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
    }
    public async Task<IList<Notification>> Get10NotificationsByRecipientId(string RecipientId, int page = 0)
    {
        return await _context.Notifications.Where(n => n.RecipientId == RecipientId && n.IsDeleted == false)
                                           .Skip(page*10)
                                           .Take(10)
                                           .ToListAsync();
    }

    public async Task<Notification> CreateNotification(Notification notification)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>{
            await _context.Notifications.AddAsync(notification);
            await SaveChanges();
            return notification;
        });
    }

    public async Task<bool> UpdateNotification(Guid id)
    {
        var existingNotification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        if(existingNotification == null)
        {
            return false;
        }
        existingNotification.IsRead = true;
        await SaveChanges();
        return true;
    }

    public async Task<bool> DeleteNotification(Guid id)
    {
        var existingNotification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        if(existingNotification == null)
        {
            return false;
        }
        existingNotification.IsDeleted = true;
        await SaveChanges();
        return true;
    }

    public async Task<IList<Notification>> GetNotificationsByUserId(string RecipientId)
    {
        return await _context.Notifications.Where(n => n.RecipientId == RecipientId).ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}