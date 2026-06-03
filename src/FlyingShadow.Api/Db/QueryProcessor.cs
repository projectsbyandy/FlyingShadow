using Dapper;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using Npgsql;

namespace FlyingShadow.Api.Db;

internal class QueryProcessor : IQueryProcessor
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public QueryProcessor(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<T, Error>> QuerySingleOrDefaultAsync<T>(string query, string? missingItemMessage, object? param = null)
    {
        return await ProcessResultAsync(async () =>
        {
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var result = await conn.QuerySingleOrDefaultAsync<T>(query, param);

            return result is null
                ? Result<T, Error>.Failure(new Error(ErrorCode.NotFound, missingItemMessage ?? "Item not found"))
                : Result<T, Error>.Success(result);
        });
    }

    public async Task<Result<IEnumerable<T>, Error>> QueryAsync<T>(string query, object? param = null)
    {
        return await ProcessAsync(async () =>
        {
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            return await conn.QueryAsync<T>(query, param);
        });
    }

    public async Task<Result<int, Error>> ExecuteAsync(string query, object? param = null)
    {
        return await ProcessAsync(async () =>
        {
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            return await conn.ExecuteAsync(query, param);
        });
    }
    
    private Task<Result<T, Error>> ProcessAsync<T>(Func<Task<T>> func)
    {
        return ProcessResultAsync(async () =>
        {
            var value = await func();
            return Result<T, Error>.Success(value);
        });
    }

    private async Task<Result<T, Error>> ProcessResultAsync<T>(Func<Task<Result<T, Error>>> func)
    {
        try
        {
            return await func();
        }
        catch (PostgresException ex)
        {
            var code = ex.SqlState switch
            {
                PostgresErrorCodes.UniqueViolation     => ErrorCode.Conflict,
                PostgresErrorCodes.ForeignKeyViolation => ErrorCode.Conflict,
                _                                      => ErrorCode.UnableToProcessData
            };
            return Result<T, Error>.Failure(new Error(code, ex.Message));
        }
        catch (NpgsqlException ex)
        {
            return Result<T, Error>.Failure(new Error(ErrorCode.DbConnectionProblem, ex.Message));
        }
    }
}