using MyMicroservice.Enums;

namespace MyMicroservice.Contracts
{
    public record PreparedRegisterJsonRequest(
        string Login,
        string Email,
        string Name,
        string PasswordHash,
        string ExtId,
        UserRoles Role
    );
}