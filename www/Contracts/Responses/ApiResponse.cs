using System.Text.Json.Serialization;

namespace MyMicroservice.Contracts.Responses;

// Success Response
public class SuccessResponse<T>(T data)
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;

    [JsonPropertyName("data")]
    public T Data { get; set; } = data;
}

// Error Response
public class ErrorResponse(string error, int errorCode)
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = false;

    [JsonPropertyName("error")]
    public string Error { get; set; } = error;

    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; set; } = errorCode;
}

// Factory для удобства
public static class ApiResponse
{
    public static SuccessResponse<T> Success<T>(T data) => new(data);

    public static ErrorResponse Error(string error, int errorCode) => new(error, errorCode);
}