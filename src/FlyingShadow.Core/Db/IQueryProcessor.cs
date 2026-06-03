using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Db;

public interface IQueryProcessor
{
    Task<Result<T, Error>> QuerySingleOrDefaultAsync<T>(string query, string? missingItemMessage = null, object? param = null);
    Task<Result<IEnumerable<T>, Error>> QueryAsync<T>(string query, object? param = null);
    Task<Result<int, Error>> ExecuteAsync(string query, object? param = null);
}