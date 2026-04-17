using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public class NotificationController : ControllerBase
{
    public readonly INotificationRepository _notificationRepository;
    public readonly IUserRepository _userRepository;
    public NotificationController(INotificationRepository notificationRepository, IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    [HttpGet("api/notifications")]
    public async Task<IActionResult> GetAllNotifications()
    {
        var notifications = await _notificationRepository.GetAll();
        if(notifications == null)
        {
            return new BadRequestResult();
        }
        return new OkResult();
    }
    [HttpGet("api/notifications/user/{userId}")]
    public async Task<IActionResult> GetNotificationsByUserId(string userId)
    {
        var notifications = await _notificationRepository.GetNotificationsByUserId(userId.ToString());
        if(notifications == null)
        {
            return new BadRequestResult();
        }
        return new OkResult();
    }
    [HttpGet("api/notifications/{id}")]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        var notification = await _notificationRepository.GetNotificationById(id);
        if(notification == null)
        {
            return new NotFoundResult();
        }
        return new OkResult();
    }
    [HttpPost("api/notifications")]
    public async Task<IActionResult> CreateNotification(Notifications notification)
    {
        await _notificationRepository.CreateNotification(notification);
        return new OkResult();
    }
    [HttpPut("api/notifications")]
    public async Task<IActionResult> UpdateNotification(Notifications notification)
    {
        await _notificationRepository.UpdateNotification(notification);
        return new OkResult();
    }
    [HttpDelete("api/notifications")]
    public async Task<IActionResult> DeleteNotification(Notifications notification)
    {
        await _notificationRepository.DeleteNotification(notification);
        return new OkResult();
    }
}