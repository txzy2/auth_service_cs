namespace MyMicroservice.Contracts.Responses;

public record AuthResponse(
    string Token,
    UserJsonResponse User
);