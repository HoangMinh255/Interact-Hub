using InteractHub.Domain.Enums;

namespace InteractHub.Application.Common;

public static class PaginationExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PagedRequest request)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(request);

        var pageNumber = request.PageNumber < 1 ? AppConstants.Pagination.DefaultPageNumber : request.PageNumber;
        var pageSize = request.PageSize < 1
            ? AppConstants.Pagination.DefaultPageSize
            : Math.Min(request.PageSize, AppConstants.Pagination.MaxPageSize);

        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static (int pageNumber, int pageSize) NormalizePaging(this PagedRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var pageNumber = request.PageNumber < 1 ? AppConstants.Pagination.DefaultPageNumber : request.PageNumber;
        var pageSize = request.PageSize < 1
            ? AppConstants.Pagination.DefaultPageSize
            : Math.Min(request.PageSize, AppConstants.Pagination.MaxPageSize);

        return (pageNumber, pageSize);
    }

    public static bool IsDescending(this PagedRequest request)
        => request.SortDirection == SortDirection.Desc;
}