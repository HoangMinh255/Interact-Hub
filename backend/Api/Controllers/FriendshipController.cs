using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.DTOs.Friendship;
using Microsoft.AspNetCore.Authorization;
using InteractHub.Application.Common;
using System.Security.Claims;
using System.Linq;

namespace InteractHub.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FriendshipController : ControllerBase
{
    private readonly IFriendshipService _friendshipService;
    public FriendshipController(IFriendshipService friendshipService)
    {
        _friendshipService = friendshipService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var friendships = await _friendshipService.GetAll();
        return Ok(friendships);
    }

    [HttpGet("friendRequests/{ReceiverId}/{page}")]
    [Authorize]
    public async Task<IActionResult> Get10FriendRequestByReceiverId(string ReceiverId, int page)
    {
        var friendRequests = await _friendshipService.Get10FriendRequestByReceiverId(ReceiverId, page);
        return Ok(friendRequests);
    }

    [HttpGet("friends/{ReceiverId}/{page}")]
    [Authorize]
    public async Task<IActionResult> Get10FriendsByReceiverId(string ReceiverId, int page)
    {
        var friends = await _friendshipService.Get10FriendsByReceiverId(ReceiverId, page);
        return Ok(friends);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateFriendRequest([FromBody] CreateFriendRequestDto createFriendRequestDto)
    {
        try{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Không thể xác thực danh tính người dùng." });
                }

                var createdFriendRequest = await _friendshipService. CreateFriendRequest(createFriendRequestDto);
                return Ok(new 
                { 
                    message = "Tạo lời mời kết bạn thành công!", 
                    createdFriendRequest
                });
        }
        catch(Exception ex)
        {
            // Log lỗi ở đây nếu cần
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo lời mời kết bạn.", error = ex.Message });
        }
    }

    [HttpPut("accept/{id}")]
    [Authorize]
    public async Task<IActionResult> AcceptFriendRequest(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var isSucceed = await _friendshipService.AcceptFriendRequest(id);
        if(isSucceed)
        {
            return Ok(new { message = "Thêm bạn thành công!"});
        }
        return NotFound(new { message = "Không tìm thấy lời mời kết bạn hoặc có lỗi khi thêm bạn!"});
    }

    [HttpPut("block/{ReceiverId}/{RequesterId}")]
    [Authorize]
    public async Task<IActionResult> BlockFriend(string RequesterId, string ReceiverId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var isSucceed = await _friendshipService.BlockFriend(RequesterId, ReceiverId);
        if(isSucceed)
        {
            return Ok(new { message = "Chặn bạn thành công!"});
        }
        return NotFound(new { message = "Không tìm thấy bạn hoặc có lỗi khi chặn bạn!"});
    }

    [HttpDelete("reject/{id}")]
    [Authorize]
    public async Task<IActionResult> RejectFriendRequest(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var isSucceed = await _friendshipService.RejectFriendRequest(id);
        if(isSucceed)
        {
            return Ok(new { message = "Từ chối thành công!"});
        }
        return NotFound(new { message = "Không tìm thấy lời mời kết bạn hoặc có lỗi khi từ chối!"});
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> RemoveFriend(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var isSucceed = await _friendshipService.RemoveFriend(id);
        if(isSucceed)
        {
            return Ok(new { message = "Hủy kết bạn thành công!"});
        }
        return NotFound(new { message = "Không tìm thấy bạn hoặc có lỗi khi hủy kết bạn!"});
    }
}