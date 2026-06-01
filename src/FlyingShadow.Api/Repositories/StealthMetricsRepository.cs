using Dapper;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class StealthMetricsRepository : IStealthMetricsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public StealthMetricsRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<IList<StealthMetrics>, Error>> GetAllAsync()
    {
        try
        {
            const string query = 
                $"""
                 SELECT *
                 FROM stealthmetrics
                 """;
        
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var allStealthMetrics = await conn.QueryAsync<StealthMetrics>(query);
            
            return Result<IList<StealthMetrics>, Error>.Success(allStealthMetrics.ToList());
        }
        catch (Exception ex)
        {
            return Result<IList<StealthMetrics>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, ex.Message));
        }
    }

    public async Task<Result<StealthMetrics, Error>> GetByShadowIdAsync(Guid id)
    {
        try
        {
            const string query = 
                $"""
                 SELECT *
                 FROM stealthmetrics
                 WHERE shadow_id = @id
                 """;
        
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var stealthMetrics = await conn.QuerySingleOrDefaultAsync<StealthMetrics>(query, new { id });

            return stealthMetrics is null
                ? Result<StealthMetrics, Error>.Failure(new Error(ErrorCode.NotFound,
                    $"StealthMetrics with ShadowId: {id} not found"))
                : Result<StealthMetrics, Error>.Success(stealthMetrics);
        }
        catch (Exception ex)
        {
            return Result<StealthMetrics, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, ex.Message));
        }
    }
}