
using InteractHub.Application.Common;
using InteractHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InteractHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HashtagController : ControllerBase
{
    private readonly IHashtagService _hashtagService;
    public HashtagController(IHashtagService hashtagService)
    {
        _hashtagService = hashtagService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var hashtags = await _hashtagService.GetAll();
        return Ok(ApiResponse.Ok("Get all hashtag successfully.", new {hashtags }));
    }

    [HttpGet("trending")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get5TrendingHashtags()
    {
        var trendingHashtags = await _hashtagService.Get5TrendingHashtags();
        return Ok(ApiResponse.Ok("Get all hashtag successfully.", new {trendingHashtags }));
    }
}