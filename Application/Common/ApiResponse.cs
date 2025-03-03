namespace Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public int StatusCode { get; init; }
    public List<string>? Errors { get; init; }

    private ApiResponse(bool success, string? message, T? data, int statusCode, List<string>? errors = null)
    {
        Success = success;
        Message = message;
        Data = success ? data : default;
        StatusCode = statusCode;
        Errors = errors ?? new List<string>();
    }

    public static ApiResponse<T> SuccessResult(T data, string message = "Success", int statusCode = 200) =>
        new(true, message, data, statusCode);

    public static ApiResponse<T> Failure(string message, int statusCode = 400, List<string>? errors = null) =>
        new(false, message, default, statusCode, errors);
}