using System.Security.Claims;
using InteractHub.Application.Common;
using InteractHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InteractHub.Api.Controllers;

[ApiController]
[Route("api/posts/{postId:guid}/likes")]
[Authorize]
public sealed class LikesController : ControllerBase
{
    private readonly ILikeService _likeService;

    public LikesController(ILikeService likeService)
    {
        _likeService = likeService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Like(Guid postId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Fail("Unauthorized"));

        await _likeService.LikeAsync(postId, userId, cancellationToken);
        var likeCount = await _likeService.GetLikeCountAsync(postId, cancellationToken);

        return Ok(ApiResponse.Ok("Post liked successfully.", new
        {
            liked = true,
            likeCount
        }));
    }

    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Unlike(Guid postId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Fail("Unauthorized"));

        await _likeService.UnlikeAsync(postId, userId, cancellationToken);
        var likeCount = await _likeService.GetLikeCountAsync(postId, cancellationToken);

        return Ok(ApiResponse.Ok("Post unliked successfully.", new
        {
            liked = false,
            likeCount
        }));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> IsLiked(Guid postId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(ApiResponse.Fail("Unauthorized"));

        var liked = await _likeService.IsLikedAsync(postId, userId, cancellationToken);

        return Ok(ApiResponse.Ok("Like state loaded successfully.", new
        {
            liked
        }));
    }

    [HttpGet("count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Count(Guid postId, CancellationToken cancellationToken)
    {
        var likeCount = await _likeService.GetLikeCountAsync(postId, cancellationToken);
        return Ok(ApiResponse.Ok("Like count loaded successfully.", new
        {
            likeCount
        }));
    }
}
