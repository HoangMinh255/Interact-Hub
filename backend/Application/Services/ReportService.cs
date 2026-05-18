using InteractHub.Application.Common;
using InteractHub.Application.Common.Exceptions;
using InteractHub.Application.DTOs.Report;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;
using InteractHub.Domain.Enums;

namespace InteractHub.Application.Services;

public sealed class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public ReportService(
        IReportRepository reportRepository,
        IPostRepository postRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository)
    {
        _reportRepository = reportRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<PostReport> CreateAsync(CreateReportDto dto, string reporterId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(reporterId))
        {
            throw new UnauthorizedException("Reporter is required.");
        }

        if (dto.PostId == Guid.Empty)
        {
            throw new BadRequestException("PostId is required.");
        }

        var reason = dto.Reason?.Trim();
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BadRequestException("Reason is required.");
        }

        var post = await _postRepository.GetPostById(dto.PostId)
            ?? throw new NotFoundException("Post not found.");

        if (string.Equals(post.UserId, reporterId, StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflictException("You cannot report your own post.");
        }

        var pendingExists = await _reportRepository.ExistsPendingForPostAsync(dto.PostId, reporterId, cancellationToken);
        if (pendingExists)
        {
            throw new ConflictException("You already have a pending report for this post.");
        }

        var report = new PostReport
        {
            PostId = dto.PostId,
            ReporterId = reporterId,
            Reason = reason,
            Status = (byte)ReportStatus.Pending
        };

        await _reportRepository.AddAsync(report, cancellationToken);
        await _reportRepository.SaveChangesAsync(cancellationToken);

        await CreateReportNotificationAsync(post.UserId, reporterId, report.Id, cancellationToken);

        return report;
    }

    public Task<PagedResult<PostReport>> GetMyReportsAsync(
        string reporterId,
        ReportQueryDto query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        return _reportRepository.GetPagedByReporterAsync(
            reporterId,
            query.Page,
            query.PageSize,
            query.Status,
            cancellationToken);
    }

    public Task<PagedResult<PostReport>> GetPendingReportsAsync(
        ReportQueryDto query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        return _reportRepository.GetPagedPendingAsync(
            query.Page,
            query.PageSize,
            cancellationToken);
    }

    public async Task<PostReport?> ReviewAsync(
        Guid reportId,
        string reviewerId,
        UpdateReportStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(reviewerId))
        {
            throw new UnauthorizedException("Reviewer is required.");
        }

        if (dto.Status == ReportStatus.Pending)
        {
            throw new BadRequestException("Review status must be Reviewed, Rejected or Resolved.");
        }

        var report = await _reportRepository.GetByIdWithDetailsAsync(reportId, cancellationToken)
            ?? throw new NotFoundException("Report not found.");

        if ((ReportStatus)report.Status != ReportStatus.Pending)
        {
            throw new ConflictException("Only pending reports can be reviewed.");
        }

        report.Status = (byte)dto.Status;
        report.ReviewedById = reviewerId;
        report.ReviewedAt = DateTime.UtcNow;
        report.UpdatedAt = DateTime.UtcNow;

        _reportRepository.Update(report);
        await _reportRepository.SaveChangesAsync(cancellationToken);

        await CreateReviewNotificationAsync(report, reviewerId, dto.Status, cancellationToken);

        return report;
    }

    public Task<PostReport?> ResolveAsync(Guid reportId, string reviewerId, CancellationToken cancellationToken = default)
    {
        return ReviewAsync(
            reportId,
            reviewerId,
            new UpdateReportStatusDto { Status = ReportStatus.Resolved },
            cancellationToken);
    }

    private async Task CreateReportNotificationAsync(
        string postOwnerId,
        string reporterId,
        Guid reportId,
        CancellationToken cancellationToken)
    {
        var reporter = await _userRepository.GetByIdAsync(reporterId, cancellationToken);
        var reporterName = reporter?.FullName?.Trim();
        if (string.IsNullOrWhiteSpace(reporterName))
        {
            reporterName = reporter?.UserName ?? "A user";
        }

        var notification = new Notification
        {
            RecipientId = postOwnerId,
            ActorId = reporterId,
            Type = (byte)NotificationType.PostReported,
            Content = $"{reporterName} reported your post.",
            RelatedEntityType = nameof(PostReport),
            RelatedEntityId = reportId.ToString(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.CreateNotification(notification);
        await _notificationRepository.SaveChanges();
    }

    private async Task CreateReviewNotificationAsync(
        PostReport report,
        string reviewerId,
        ReportStatus status,
        CancellationToken cancellationToken)
    {
        var reporter = await _userRepository.GetByIdAsync(report.ReporterId, cancellationToken);
        var reporterName = reporter?.FullName?.Trim();
        if (string.IsNullOrWhiteSpace(reporterName))
        {
            reporterName = reporter?.UserName ?? "A user";
        }

        var content = status switch
        {
            ReportStatus.Rejected => $"Your report for post {report.PostId} was rejected.",
            ReportStatus.Resolved => $"Your report for post {report.PostId} was resolved.",
            ReportStatus.Reviewed => $"Your report for post {report.PostId} was reviewed.",
            _ => $"Your report for post {report.PostId} was updated."
        };

        var notification = new Notification
        {
            RecipientId = report.ReporterId,
            ActorId = reviewerId,
            Type = (byte)NotificationType.ReportReviewed,
            Content = content,
            RelatedEntityType = nameof(PostReport),
            RelatedEntityId = report.Id.ToString(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.CreateNotification(notification);
        await _notificationRepository.SaveChanges();
    }
}
