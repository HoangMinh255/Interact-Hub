using System.Security.Claims;
using InteractHub.Application.Common;
using InteractHub.Application.DTOs.User;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InteractHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Ok("Unauthorized"));
        }

        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound(ApiResponse.Ok("User not found."));
        }

        return Ok(ApiResponse.Ok("User profile loaded successfully.", MapToProfileDto(user)));
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(string userId, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound(ApiResponse.Ok("User not found."));
        }

        return Ok(ApiResponse.Ok("User profile loaded successfully.", MapToProfileDto(user)));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] UserSearchQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _userService.SearchAsync(query, cancellationToken);
        var mapped = new PagedResult<UserSummaryDto>
        {
            Items = result.Items.Select(MapToSummaryDto).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse.Ok("Users loaded successfully.", mapped));
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserProfileDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Ok("Unauthorized"));
        }

        var user = await _userService.UpdateProfileAsync(userId, dto, cancellationToken);
        return Ok(ApiResponse.Ok("Profile updated successfully.", MapToProfileDto(user)));
    }

    [HttpPost("me/avatar")]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Ok("Unauthorized"));
        }

        if (request.AvatarFile is null || request.AvatarFile.Length == 0)
        {
            return BadRequest(ApiResponse.Ok("Avatar file is required."));
        }

        if (!AllowedImageContentTypes.Contains(request.AvatarFile.ContentType))
        {
            return BadRequest(ApiResponse.Ok("Unsupported avatar image type."));
        }

        await using var stream = request.AvatarFile.OpenReadStream();
        var user = await _userService.UploadAvatarAsync(
            userId,
            stream,
            request.AvatarFile.FileName,
            request.AvatarFile.ContentType,
            cancellationToken);

        return Ok(ApiResponse.Ok("Avatar uploaded successfully.", MapToProfileDto(user)));
    }

    [HttpPatch("{userId}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Deactivate(string userId, CancellationToken cancellationToken)
    {
        var isSuccess = await _userService.DeactivateAsync(userId, cancellationToken);
        if (!isSuccess)
        {
            return Ok(ApiResponse.Ok("User is already inactive."));
        }

        return Ok(ApiResponse.Ok("User deactivated successfully."));
    }

    [HttpPatch("{userId}/reactivate")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reactivate(string userId, CancellationToken cancellationToken)
    {
        var isSuccess = await _userService.ReactivateAsync(userId, cancellationToken);
        if (!isSuccess)
        {
            return Ok(ApiResponse.Ok("User is already active."));
        }

        return Ok(ApiResponse.Ok("User reactivated successfully."));
    }

    private string? GetCurrentUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private static UserProfileResponseDto MapToProfileDto(ApplicationUser user)
        => new()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

    private static UserSummaryDto MapToSummaryDto(ApplicationUser user)
        => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            IsActive = user.IsActive
        };
}
