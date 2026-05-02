using InteractHub.Application.Common;
using InteractHub.Domain.Entities;
using InteractHub.Domain.Enums;

namespace InteractHub.Application.Interfaces.Repositories;

public interface IReportRepository
{
    Task<PostReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PostReport?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsPendingForPostAsync(Guid postId, string reporterId, CancellationToken cancellationToken = default);
    Task<PagedResult<PostReport>> GetPagedByReporterAsync(
        string reporterId,
        int page,
        int pageSize,
        ReportStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<PagedResult<PostReport>> GetPagedPendingAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(PostReport report, CancellationToken cancellationToken = default);
    void Update(PostReport report);
    void Remove(PostReport report);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
