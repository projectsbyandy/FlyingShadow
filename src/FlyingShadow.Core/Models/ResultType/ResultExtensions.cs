namespace FlyingShadow.Core.Models.ResultType;

public static class ResultExtensions
{
    /// <summary>
    /// Chains a sync step onto an already-resolved result.
    /// </summary>
    /// <param name="result">The resolved result to bind from.</param>
    /// <param name="next">The sync function to invoke if the result is successful.</param>
    /// <returns>The result of invoking <paramref name="next"/>, or a failure propagated from <paramref name="result"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;User, Error&gt; result = GetUser(id)
    ///     .Bind(user => ValidateUser(user));
    /// </code>
    /// </example>

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
    /// <param name="resultTask">The pending task result to await and bind from.</param>
    /// <param name="next">The sync function to invoke if the result is successful.</param>
    /// <returns>The result of invoking <paramref name="next"/>, or a failure propagated from <paramref name="resultTask"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;User, Error&gt; result = await GetUserAsync(id)
    ///     .Bind(user => ValidateUser(user));
    /// </code>
    /// </example>
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
    /// <param name="result">The resolved result to bind from.</param>
    /// <param name="next">The async function to invoke if the result is successful.</param>
    /// <returns>The result of invoking <paramref name="next"/>, or a failure propagated from <paramref name="result"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;Order, Error&gt; result = await GetUser(id)
    ///     .BindAsync(user => GetOrderAsync(user));
    /// </code>
    /// </example>
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
    /// <param name="resultTask">The pending task result to await and bind from.</param>
    /// <param name="next">The async function to invoke if the result is successful.</param>
    /// <returns>The result of invoking <paramref name="next"/>, or a failure propagated from <paramref name="resultTask"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;Order, Error&gt; result = await GetUserAsync(id)
    ///     .BindAsync(user => GetOrderAsync(user));
    /// </code>
    /// </example>
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