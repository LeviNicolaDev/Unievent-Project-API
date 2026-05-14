using Microsoft.AspNetCore.Http;

namespace Unievent.Application.Common;


public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;

    public T? Value { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result<T> Success(T value)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Value = value
        };
    }


    public static Result<T> Failure(string error)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = new List<string> { error }
        };
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}

