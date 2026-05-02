using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
<<<<<<< HEAD
using InteractHub.Persistence.Repositories;
using InteractHub.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
=======
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.DTOs.Notification;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
>>>>>>> origin/feature/frontend-mobile-friendly
namespace InteractHub.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
<<<<<<< HEAD
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
=======
    public readonly INotificationService _notificationService;
    public NotificationController(INotificationService notificationRepository)
    {
        _notificationService = notificationRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllNotifications()
    {
        var notifications = await _notificationService.GetAllNotifications();
>>>>>>> origin/feature/frontend-mobile-friendly
        if(notifications == null)
        {
            return new BadRequestResult();
        }
<<<<<<< HEAD
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
=======
        return Ok(notifications);
    }

    [HttpGet("user/{RecipientId}/page/{page}")]
    [Authorize]
    public async Task<IActionResult> Get10NotificationsByRecipientId(string RecipientId, int page)
    {
        try{
            var notifications = await _notificationService.Get10NotificationsByRecipientId(RecipientId,page);
            if(notifications == null)
            {
                return BadRequest(new { message = "Thông báo phải có nội dung." });
            }
            return Ok(notifications);
        }
        catch(Exception e)
        {
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống: ", error = e.Message });
        }
        
>>>>>>> origin/feature/frontend-mobile-friendly
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
<<<<<<< HEAD
        var notification = await _notificationRepository.GetNotificationById(id);
        if(notification == null)
        {
            return new NotFoundResult();
        }
        return new OkResult();
=======
        var notification = await _notificationService.GetNotificationById(id);
        if(notification == null)
        {
            return NotFound(new {message = "Không tìm thấy thông báo!"});
        }
        return Ok(notification);
>>>>>>> origin/feature/frontend-mobile-friendly
    }

    [Authorize]
    [HttpPost]
<<<<<<< HEAD
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
=======
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto notification)
    {
        // Lấy UserId từ token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var createdNotification = await _notificationService.CreateNotification(notification);
        return Ok(createdNotification);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNotification(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        

        var isSucced = await _notificationService.UpdateNotification(id);
        if(isSucced)
        {
            return Ok(new { message = "Cập nhật thông báo thành công!"});
        }
        return NotFound(new { message = "Không tìm thấy thông báo hoặc có lỗi khi cập nhật!"});
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var isSucced = await _notificationService.UpdateNotification(id);
        if(isSucced)
        {
            return Ok(new { message = "Xóa thông báo thành công!"});
        }
        return NotFound(new { message = "Không tìm thấy thông báo hoặc có lỗi khi xóa!"});
>>>>>>> origin/feature/frontend-mobile-friendly
    }
}