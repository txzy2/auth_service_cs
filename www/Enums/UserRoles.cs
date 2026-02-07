namespace MyMicroservice.Enums;

public enum UserRoles
{
    Admin = 1,
    User = 2,
    Moderator = 3
}

public static class UserRolesExtensions
{
    public static string ToAlias(this UserRoles role)
    {
        return role switch
        {
            UserRoles.User => "user",
            UserRoles.Admin => "admin",
            UserRoles.Moderator => "moderator",
            _ => "unknown"
        };
    }

    // Безопасная версия (возвращает null при ошибке)
    public static UserRoles? TryFromAlias(string alias)
    {
        return alias?.ToLower() switch
        {
            "user" => UserRoles.User,
            "admin" => UserRoles.Admin,
            "moderator" => UserRoles.Moderator,
            _ => null
        };
    }

    public static int ToRoleId(this UserRoles role)
    {
        return (int)role;
    }

    public static string? FromRoleId(int roleId)
    {
        return roleId switch
        {
            1 => UserRoles.Admin.ToAlias(),
            2 => UserRoles.User.ToAlias(),
            3 => UserRoles.Moderator.ToAlias(),
            _ => null
        };
    }
}