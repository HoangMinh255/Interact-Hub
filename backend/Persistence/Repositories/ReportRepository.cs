using InteractHub.Application.Common;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Domain.Entities;
using InteractHub.Domain.Enums;
using InteractHub.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace InteractHub.Persistence.Repositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<PostReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Set<PostReport>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<PostReport?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Set<PostReport>()
            .Include(x => x.Post)
            .Include(x => x.Reporter)
            .Include(x => x.ReviewedBy)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> ExistsPendingForPostAsync(Guid postId, string reporterId, CancellationToken cancellationToken = default)
    {
        return _context.Set<PostReport>()
            .AnyAsync(x =>
                x.PostId == postId &&
                x.ReporterId == reporterId &&
                x.Status == (byte)ReportStatus.Pending,
                cancellationToken);
    }

    public async Task<PagedResult<PostReport>> GetPagedByReporterAsync(
        string reporterId,
        int page,
        int pageSize,
        ReportStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<PostReport> query = _context.Set<PostReport>()
            .AsNoTracking()
            .Include(x => x.Post)
            .Include(x => x.Reporter)
            .Include(x => x.ReviewedBy)
            .Where(x => x.ReporterId == reporterId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == (byte)status.Value);
        }

        var totalItems = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResult<PostReport>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<PagedResult<PostReport>> GetPagedPendingAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Set<PostReport>()
            .AsNoTracking()
            .Include(x => x.Post)
            .Include(x => x.Reporter)
            .Include(x => x.ReviewedBy)
            .Where(x => x.Status == (byte)ReportStatus.Pending);

        var totalItems = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResult<PostReport>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task AddAsync(PostReport report, CancellationToken cancellationToken = default)
    {
        await _context.Set<PostReport>().AddAsync(report, cancellationToken);
    }

    public void Update(PostReport report)
    {
        _context.Set<PostReport>().Update(report);
    }

    public void Remove(PostReport report)
    {
        _context.Set<PostReport>().Remove(report);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
