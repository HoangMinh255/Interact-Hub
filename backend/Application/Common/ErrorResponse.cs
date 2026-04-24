namespace InteractHub.Application.Common;

public class ErrorResponse
{
    public string Message { get; set; } = "Validation failed";
    public List<string> Errors { get; set; } = [];
    public string? TraceId { get; set; }

    public static ErrorResponse Create(string message, IEnumerable<string> errors, string? traceId = null)
        => new()
        {
            Message = message,
            Errors = errors.ToList(),
            TraceId = traceId
        };
}