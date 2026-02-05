using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace MyMicroservice.Contracts.Requests;

public record RegisterJsonRequest(
    [property: JsonPropertyName("login")]
    [Required]
    [MinLength(3)]
    string Login,

    [property: JsonPropertyName("password")]
    [Required]
    [MinLength(8)]
    string Password,

    [property: JsonPropertyName("demo")]
    [Optional]
    string? Demo
);