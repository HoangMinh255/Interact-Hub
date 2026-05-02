using InteractHub.Application.Common;
using InteractHub.Application.DTOs.Report;
using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Services;

public interface IReportService
{
    Task<PostReport> CreateAsync(CreateReportDto dto, string reporterId, CancellationToken cancellationToken = default);
    Task<PagedResult<PostReport>> GetMyReportsAsync(string reporterId, ReportQueryDto query, CancellationToken cancellationToken = default);
    Task<PagedResult<PostReport>> GetPendingReportsAsync(ReportQueryDto query, CancellationToken cancellationToken = default);
    Task<PostReport?> ReviewAsync(Guid reportId, string reviewerId, UpdateReportStatusDto dto, CancellationToken cancellationToken = default);
    Task<PostReport?> ResolveAsync(Guid reportId, string reviewerId, CancellationToken cancellationToken = default);
}
