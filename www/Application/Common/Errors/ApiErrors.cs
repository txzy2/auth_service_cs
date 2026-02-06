namespace MyMicroservice.Application.Common.Errors;

public enum ApiError
{
    InternalError,
    UserNotFound,
    UserExist,
    NullList,
    InvalidCredentials,
    ValidationError,
    InvalidRole
}