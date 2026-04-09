using Microsoft.AspNetCore.Http;

namespace Unievent.Application.Common;

public class Result
{
    public Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string Message { get; private set; }

    public static Result Success(string message = "") => new(true, message);
    public static Result Failure(string message) => new(false, message);
}

public class ResultData<T> : Result
{
    public ResultData(T? data, bool isSuccess = true, string message = "") : base(isSuccess, message)
    {
        Data = data;
    }
    public T? Data { get; private set; }

    public static ResultData<T> Success(T data) => new(data);
    public static ResultData<T> Failure(string message) => new(default, false, message);
}