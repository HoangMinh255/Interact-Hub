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

    public async Task<IActionResult> CreateNotification(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await SaveChanges();
        return new OkResult();
    }

    public async Task<IActionResult> UpdateNotification(Notification notification)
    {
        _context.Notifications.Update(notification);
        await SaveChanges();
        return new OkResult();
    }

    public async Task<IActionResult> DeleteNotification(Notification notification)
    {
        _context.Notifications.Remove(notification);
        await SaveChanges();
        return new OkResult();
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