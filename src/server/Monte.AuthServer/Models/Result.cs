namespace Monte.AuthServer.Models;

public class Result
{
    private static readonly Result SuccessfulResult = new();

    public string? ErrorMessage { get; }
    public ErrorType ErrType { get; }

    protected Result(string? error, ErrorType type)
    {
        ErrorMessage = error;
        ErrType = type;
    }

    protected Result()
    {
        ErrorMessage = string.Empty;
        ErrType = ErrorType.None;
    }

    public static Result Success()
        => SuccessfulResult;

    public static Result Failure(ErrorType error, string? message = null)
        => new(message, error);

    public static Result<T> Success<T>(T? value)
        => Result<T>.Success(value);

    public static Result<T> Failure<T>(ErrorType error, string? message = null)
        => Result<T>.Failure(error, message);
}

public class Result<T> : Result
{
    public T? Object { get; }

    protected Result(T? obj)
    {
        Object = obj;
    }

    protected Result(string? error, ErrorType type) : base(error, type)
    {
    }

    public static Result<T> Success(T? value)
        => new(value);

    public static new Result<T> Failure(ErrorType error, string? message = null)
        => new(message, error);
}
