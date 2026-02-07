using MyMicroservice.Enums;

namespace MyMicroservice.Domain.ValueObjects;

public record PreparedRegisterJsonRequest(
    string Login,
    string Email,
    string Name,
    string PasswordHash,
    string ExtId,
    UserRoles Role
);