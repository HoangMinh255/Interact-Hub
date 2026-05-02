using System.Security.Claims;
using InteractHub.Application.Common;
using InteractHub.Application.DTOs.Story;
using InteractHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InteractHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class StoriesController : ControllerBase
{
    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private static readonly HashSet<string> AllowedVideoContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "video/mp4",
        "video/webm",
        "video/quicktime"
    };

    private readonly IStoryService _storyService;
    private readonly IFileStorageService _fileStorageService;

    public StoriesController(
        IStoryService storyService,
        IFileStorageService fileStorageService)
    {
        _storyService = storyService;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StoryResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyStories(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Ok("Unauthorized"));

        var stories = await _storyService.GetActiveStoriesByUserIdAsync(userId, cancellationToken);
        var result = stories.Select(MapToDto).ToList();

        return Ok(ApiResponse.Ok("My stories loaded successfully.", result));
    }

    [HttpGet("feed")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StoryResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Ok("Unauthorized"));

        var stories = await _storyService.GetFeedAsync(userId, page, pageSize, cancellationToken);
        var result = stories.Select(MapToDto).ToList();

        return Ok(ApiResponse.Ok("Story feed loaded successfully.", result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateStoryDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Ok("Unauthorized"));

        var story = await _storyService.CreateAsync(dto, userId, cancellationToken);
        return Ok(ApiResponse.Ok("Story created successfully.", MapToDto(story)));
    }

    [HttpPost("upload")]
    [RequestSizeLimit(50_000_000)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<StoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Upload([FromForm] CreateStoryWithFileRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Ok("Unauthorized"));

        if (request.MediaFile is null || request.MediaFile.Length == 0)
            return BadRequest(ApiResponse.Ok("Media file is required."));

        if (!IsAllowedContentType(request.MediaFile.ContentType))
            return BadRequest(ApiResponse.Ok("Unsupported media type."));

        await using var stream = request.MediaFile.OpenReadStream();
        var (blobName, url) = await _fileStorageService.UploadAsync(
            stream,
            request.MediaFile.FileName,
            request.MediaFile.ContentType,
            "stories",
            cancellationToken);

        var dto = new CreateStoryDto
        {
            Content = request.Content,
            MediaUrl = url,
            MediaType = request.MediaFile.ContentType,
            ExpireAt = request.ExpireAt
        };

        var story = await _storyService.CreateAsync(dto, userId, cancellationToken);
        return Ok(ApiResponse.Ok("Story uploaded successfully.", MapToDto(story)));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStoryDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Ok("Unauthorized"));

        var story = await _storyService.UpdateAsync(id, userId, dto, cancellationToken);
        if (story is null)
            return NotFound(ApiResponse.Ok("Story not found or you do not have permission."));

        return Ok(ApiResponse.Ok("Story updated successfully.", MapToDto(story)));
    }

    [HttpPatch("{id:guid}/hide")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Hide(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Ok("Unauthorized"));

        var isSuccess = await _storyService.HideAsync(id, userId, cancellationToken);
        if (!isSuccess)
            return NotFound(ApiResponse.Ok("Story not found or you do not have permission."));

        return Ok(ApiResponse.Ok("Story hidden successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Ok("Unauthorized"));

        var isSuccess = await _storyService.DeleteAsync(id, userId, cancellationToken);
        if (!isSuccess)
            return NotFound(ApiResponse.Ok("Story not found or you do not have permission."));

        return Ok(ApiResponse.Ok("Story deleted successfully."));
    }

    private string? GetCurrentUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private static StoryResponseDto MapToDto(Domain.Entities.Story story)
        => new()
        {
            Id = story.Id,
            UserId = story.UserId,
            Content = story.Content,
            MediaUrl = story.MediaUrl,
            MediaType = story.MediaType,
            ExpireAt = story.ExpireAt,
            IsActive = story.IsActive,
            CreatedAt = story.CreatedAt
        };

    private static bool IsAllowedContentType(string? contentType)
        => !string.IsNullOrWhiteSpace(contentType)
           && (AllowedImageContentTypes.Contains(contentType) || AllowedVideoContentTypes.Contains(contentType));
}
