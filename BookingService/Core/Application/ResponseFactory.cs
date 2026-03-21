using System;

namespace Application;

public static class ResponseFactory
{
    public static T Fail<T>(FailureInfo failure)
        where T : Response, new()
    {
        return new T
        {
            Success = false,
            ErrorCode = failure.ErrorCode,
            Message = failure.Message
        };
    }

    public static T Ok<T>(Action<T> configure)
        where T : Response, new()
    {
        var response = new T { Success = true };
        configure(response);
        return response;
    }
}
