using System.Diagnostics.CodeAnalysis;

namespace FlyingShadow.Core.Models.ResultType;

public class Result<T, TError>
{
    public T? Value { get; }
    public TError? Error { get; }
    
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }
    
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;
 
    private Result(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));;
        IsSuccess = true;
    }
 
    private Result(TError error)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));;
        IsSuccess = false;
    }
 
    public static Result<T, TError> Success(T value) => new(value);
    public static Result<T, TError> Failure(TError error) => new(error);
}