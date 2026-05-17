namespace InteractHub.Application.Common;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public List<string> Errors { get; set; } = [];
    public string? TraceId { get; set; }

    public static ApiResponse Ok(string message = "Operation completed successfully", object? data = null, string? traceId = null)
        => new()
        {
            Success = true,
            Message = message,
            Data = data,
            TraceId = traceId
        };

    public static ApiResponse Fail(string message = "Operation failed", IEnumerable<string>? errors = null, string? traceId = null)
        => new()
        {
            Success = false,
            Message = message,
            Errors = errors?.ToList() ?? [],
            TraceId = traceId
        };
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];
    public string? TraceId { get; set; }

    public static ApiResponse<T> Ok(T? data, string message = "Operation completed successfully", string? traceId = null)
        => new()
        {
            Success = true,
            Message = message,
            Data = data,
            TraceId = traceId
        };

    public static ApiResponse<T> Fail(string message = "Operation failed", IEnumerable<string>? errors = null, string? traceId = null)
        => new()
        {
            Success = false,
            Message = message,
            Errors = errors?.ToList() ?? [],
            Data = default,
            TraceId = traceId
        };
}