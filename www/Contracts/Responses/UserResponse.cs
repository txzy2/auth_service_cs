namespace MyMicroservice.Contracts.Responses;

public record UserJsonResponse(
    string Login,
    string Email,
    string Name,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt
);