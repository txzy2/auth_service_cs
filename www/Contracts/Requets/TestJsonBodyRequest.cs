using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyMicroservice.Contracts.Requests;

public record TestJsonBodyRequest(
    [property: JsonPropertyName("name")]
    [Required]
    string Name
);
