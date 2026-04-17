namespace FlyingShadow.Core.Models.ResultType;

public static class ResultMapExtensions
{
    /// <summary>
    /// Transforms the value of an already-resolved result.
    /// Used when the transformation cannot fail and returns a plain value rather than a Result.
    /// </summary>
    /// <param name="result">The resolved result to map from.</param>
    /// <param name="mapper">The sync function to invoke if the result is successful.</param>
    /// <returns>A new successful result containing the mapped value, or a failure propagated from <paramref name="result"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;ShadowDto, Error&gt; result = GetShadow(codeName)
    ///     .Map(shadow => _mapper.ToSingle(shadow));
    /// </code>
    /// </example>
    public static Result<TNext, TError> Map<T, TNext, TError>(
        this Result<T, TError> result,
        Func<T, TNext> mapper)
        => result.IsSuccess
            ? Result<TNext, TError>.Success(mapper(result.Value!))
            : Result<TNext, TError>.Failure(result.Error!);

    /// <summary>
    /// Transforms the value of a pending task result.
    /// Allows a sync transformation to follow an async step without breaking the chain.
    /// </summary>
    /// <param name="resultTask">The pending task result to await and map from.</param>
    /// <param name="mapper">The sync function to invoke if the result is successful.</param>
    /// <returns>A new successful result containing the mapped value, or a failure propagated from <paramref name="resultTask"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;ShadowDto, Error&gt; result = await GetShadowAsync(codeName)
    ///     .Map(shadow => _mapper.ToSingle(shadow));
    /// </code>
    /// </example>
    public static async Task<Result<TNext, TError>> Map<T, TNext, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<T, TNext> mapper)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? Result<TNext, TError>.Success(mapper(result.Value!))
            : Result<TNext, TError>.Failure(result.Error!);
    }

    /// <summary>
    /// Transforms the value of an already-resolved result using an async mapper.
    /// Used when the transformation itself is async but cannot fail.
    /// </summary>
    /// <param name="result">The resolved result to map from.</param>
    /// <param name="mapper">The async function to invoke if the result is successful.</param>
    /// <returns>A new successful result containing the mapped value, or a failure propagated from <paramref name="result"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;ShadowDto, Error&gt; result = await GetShadow(codeName)
    ///     .MapAsync(shadow => _mapper.ToSingleAsync(shadow));
    /// </code>
    /// </example>
    public static async Task<Result<TNext, TError>> MapAsync<T, TNext, TError>(
        this Result<T, TError> result,
        Func<T, Task<TNext>> mapper)
        => result.IsSuccess
            ? Result<TNext, TError>.Success(await mapper(result.Value!))
            : Result<TNext, TError>.Failure(result.Error!);

    /// <summary>
    /// Transforms the value of a pending task result using an async mapper.
    /// Used when both the preceding step and the transformation are async.
    /// </summary>
    /// <param name="resultTask">The pending task result to await and map from.</param>
    /// <param name="mapper">The async function to invoke if the result is successful.</param>
    /// <returns>A new successful result containing the mapped value, or a failure propagated from <paramref name="resultTask"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;ShadowDto, Error&gt; result = await GetShadowAsync(codeName)
    ///     .MapAsync(shadow => _mapper.ToSingleAsync(shadow));
    /// </code>
    /// </example>
    public static async Task<Result<TNext, TError>> MapAsync<T, TNext, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<TNext>> mapper)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? Result<TNext, TError>.Success(await mapper(result.Value!))
            : Result<TNext, TError>.Failure(result.Error!);
    }
}