namespace FlyingShadow.Api.DTO.ResultType;

public class Result<T>
{
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess  => Error is null;

    private Result(T value) => Value = value;
    private Result(Error error) => Error = error;

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
    public static Result<TOut> Bind<TIn, TOut>(
        Result<TIn> result,
        Func<TIn, Result<TOut>> func)
        => result.IsSuccess ? func(result.Value!) : Result<TOut>.Failure(result.Error!);
}
