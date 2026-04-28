using System.Diagnostics.CodeAnalysis;

namespace FlyingShadow.Core.Models.ResultType;

public class Result<T, TError>
{
    public T Value => IsSuccess
        ? field!
        : throw new InvalidOperationException(
            "Cannot access Value on a failed Result. Check IsSuccess before reading Value.");

    public TError Error => IsFailure
        ? field!
        : throw new InvalidOperationException(
            "Cannot access Error on a successful Result. Check IsFailure before reading Error.");

    
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }
    
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;
 
    private Result(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsSuccess = true;
    }
 
    private Result(TError error)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
        IsSuccess = false;
    }
 
    public static Result<T, TError> Success(T value) => new(value);
    public static Result<T, TError> Failure(TError error) => new(error);
}