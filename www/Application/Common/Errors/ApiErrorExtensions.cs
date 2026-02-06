namespace MyMicroservice.Application.Common.Errors;

public static class ApiErrorExtensions
{
    public static string GetMessage(this ApiError error)
    {
        return error switch
        {
            ApiError.InternalError => "Internal server error",
            ApiError.UserNotFound => "User not found",
            ApiError.UserExist => "User already exists",
            ApiError.NullList => "User list is null",
            ApiError.InvalidCredentials => "Invalid credentials",
            ApiError.ValidationError => "Validation error",
            ApiError.InvalidRole => "Invalid user role",
            _ => throw new ArgumentOutOfRangeException(nameof(error), error, null)
        };
    }

    public static int GetStatusCode(this ApiError error)
    {
        return error switch
        {
            ApiError.InternalError => 500,
            ApiError.UserNotFound => 404,
            ApiError.UserExist => 409,
            ApiError.NullList => 404,
            ApiError.InvalidCredentials => 401,
            ApiError.ValidationError or ApiError.InvalidRole => 400,
            _ => 500
        };
    }
}