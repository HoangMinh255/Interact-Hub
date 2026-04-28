using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Persistence.Repositories;
using InteractHub.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
namespace InteractHub.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    public readonly INotificationRepository _notificationRepository;
    public NotificationController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllNotifications()
    {
        var notifications = await _notificationRepository.GetAll();
        if(notifications == null)
        {
            return new BadRequestResult();
        }
        return new OkResult();
    }

    [Authorize]
    [HttpGet("user/{RecipientId}")]
    public async Task<IActionResult> GetNotificationsByUserId(string RecipientId)
    {
        var notifications = await _notificationRepository.GetNotificationsByUserId(RecipientId);
        if(notifications == null)
        {
            return new BadRequestResult();
        }
        return new OkResult();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        var notification = await _notificationRepository.GetNotificationById(id);
        if(notification == null)
        {
            return new NotFoundResult();
        }
        return new OkResult();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNotification(Notification notification)
    {
        await _notificationRepository.CreateNotification(notification);
        return new OkResult();
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateNotification(Notification notification)
    {
        await _notificationRepository.UpdateNotification(notification);
        return new OkResult();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteNotification(Notification notification)
    {
        await _notificationRepository.DeleteNotification(notification);
        return new OkResult();
    }
}