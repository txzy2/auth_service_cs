
using MyMicroservice.Application.Common.Errors;

namespace MyMicroservice.Application.Common.Exceptions;

public class ApiException : Exception
{
    public ApiError Error { get; }
    public int StatusCode { get; }

    public ApiException(ApiError error) : base(error.GetMessage())
    {
        Error = error;
        StatusCode = error.GetStatusCode();
    }

    public ApiException(ApiError error, string customMessage) : base(customMessage)
    {
        Error = error;
        StatusCode = error.GetStatusCode();
    }
}