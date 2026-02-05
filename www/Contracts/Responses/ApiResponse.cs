using System.Text.Json.Serialization;

namespace MyMicroservice.Contracts.Responses;

public record ApiResponse<T>(
    bool Success,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // Убираем поле из JSON, если оно null
    [property: JsonPropertyName("data")]
    T? Data,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? ErrorMessage = null
)
{
    public static ApiResponse<T> Ok(T? Data) => new(true, Data);
    public static ApiResponse<T> Error(string? ErrorMessage = null) => new(false, default, ErrorMessage);
}
