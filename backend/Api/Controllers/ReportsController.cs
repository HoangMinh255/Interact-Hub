using System.Security.Claims;
using InteractHub.Application.Common;
using InteractHub.Application.DTOs.Report;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InteractHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReportResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateReportDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Ok("Unauthorized"));
        }

        var report = await _reportService.CreateAsync(dto, userId, cancellationToken);
        return Ok(ApiResponse.Ok("Report created successfully.", MapToDto(report)));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReportResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReports([FromQuery] ReportQueryDto query, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Ok("Unauthorized"));
        }

        var result = await _reportService.GetMyReportsAsync(userId, query, cancellationToken);
        return Ok(ApiResponse.Ok("My reports loaded successfully.", MapPaged(result)));
    }

    [HttpGet("pending")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReportResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending([FromQuery] ReportQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetPendingReportsAsync(query, cancellationToken);
        return Ok(ApiResponse.Ok("Pending reports loaded successfully.", MapPaged(result)));
    }

    [HttpPatch("{reportId:guid}/review")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<ReportResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Review(Guid reportId, [FromBody] UpdateReportStatusDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Ok("Unauthorized"));
        }

        var report = await _reportService.ReviewAsync(reportId, userId, dto, cancellationToken);
        if (report is null)
        {
            return NotFound(ApiResponse.Ok("Report not found."));
        }

        return Ok(ApiResponse.Ok("Report reviewed successfully.", MapToDto(report)));
    }

    [HttpPatch("{reportId:guid}/resolve")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<ReportResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Resolve(Guid reportId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Ok("Unauthorized"));
        }

        var report = await _reportService.ResolveAsync(reportId, userId, cancellationToken);
        if (report is null)
        {
            return NotFound(ApiResponse.Ok("Report not found."));
        }

        return Ok(ApiResponse.Ok("Report resolved successfully.", MapToDto(report)));
    }

    private string? GetCurrentUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private static ReportResponseDto MapToDto(PostReport report)
        => new()
        {
            Id = report.Id,
            PostId = report.PostId,
            ReporterId = report.ReporterId,
            ReporterName = report.Reporter?.FullName ?? report.Reporter?.UserName,
            Reason = report.Reason,
            Status = (Domain.Enums.ReportStatus)report.Status,
            ReviewedById = report.ReviewedById,
            ReviewedByName = report.ReviewedBy?.FullName ?? report.ReviewedBy?.UserName,
            ReviewedAt = report.ReviewedAt,
            CreatedAt = report.CreatedAt
        };

    private static PagedResult<ReportResponseDto> MapPaged(PagedResult<PostReport> result)
        => new()
        {
            Items = result.Items.Select(MapToDto).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
}
