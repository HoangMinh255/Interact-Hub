using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.DTOs.Notification;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
namespace InteractHub.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
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
        if(notifications == null)
        {
            return new BadRequestResult();
        }
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
        
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        var notification = await _notificationService.GetNotificationById(id);
        if(notification == null)
        {
            return NotFound(new {message = "Không tìm thấy thông báo!"});
        }
        return Ok(notification);
    }

    [Authorize]
    [HttpPost]
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
    }
}