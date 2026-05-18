using System.ComponentModel.DataAnnotations;
using InteractHub.Domain.Enums;

namespace InteractHub.Application.Common;

public class PagedRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0.")]
    public int PageNumber { get; set; } = AppConstants.Pagination.DefaultPageNumber;

    [Range(1, AppConstants.Pagination.MaxPageSize, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = AppConstants.Pagination.DefaultPageSize;

    [MaxLength(100)]
    public string? SortBy { get; set; }

    public SortDirection SortDirection { get; set; } = SortDirection.Desc;

    [MaxLength(200)]
    public string? Keyword { get; set; }
}