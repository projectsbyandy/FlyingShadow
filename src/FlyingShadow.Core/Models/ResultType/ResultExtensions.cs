namespace FlyingShadow.Core.Models.ResultType;

public static class ResultExtensions
{
    /// <summary>
    /// Chains a sync step onto an already-resolved result.
    /// </summary>
    public static Result<TNext, TError> Bind<T, TNext, TError>(
        this Result<T, TError> result,
        Func<T, Result<TNext, TError>> next)
        => result.IsSuccess
            ? next(result.Value!)
            : Result<TNext, TError>.Failure(result.Error!);
 
    /// <summary>
    /// Chains a sync step onto a pending task result.
    /// Allows a sync step to follow an async one without breaking the chain.
    /// </summary>
    public static async Task<Result<TNext, TError>> Bind<T, TNext, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Result<TNext, TError>> next)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? next(result.Value!)
            : Result<TNext, TError>.Failure(result.Error!);
    }

    /// <summary>
    /// Chains an async step onto an already-resolved result.
    /// Used for the first async step in the pipeline.
    /// </summary>
    public static async Task<Result<TNext, TError>> BindAsync<T, TNext, TError>(
        this Result<T, TError> result,
        Func<T, Task<Result<TNext, TError>>> next)
        => result.IsSuccess
            ? await next(result.Value!)
            : Result<TNext, TError>.Failure(result.Error!);
 
    /// <summary>
    /// Chains an async step onto a pending task result.
    /// Used for every subsequent async step in the pipeline.
    /// </summary>
    public static async Task<Result<TNext, TError>> BindAsync<T, TNext, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<Result<TNext, TError>>> next)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await next(result.Value!)
            : Result<TNext, TError>.Failure(result.Error!);
    }
}