using System.Diagnostics.CodeAnalysis;

namespace FlyingShadow.Core.Models.ResultType;

public class Result<T, TError>
{
    public T? Value { get; }
    public TError? Error { get; }
    
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; } 
 
    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }
 
    private Result(TError error)
    {
        Error = error;
        IsSuccess = false;
    }
 
    public static Result<T, TError> Success(T value) => new(value);
    public static Result<T, TError> Failure(TError error) => new(error);
}