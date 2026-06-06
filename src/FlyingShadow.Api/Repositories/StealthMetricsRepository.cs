using Dapper;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class StealthMetricsRepository : IStealthMetricsRepository
{
    private readonly IQueryProcessor _queryProcessor;

    public StealthMetricsRepository(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    public async Task<Result<IEnumerable<StealthMetrics>, Error>> GetAllAsync()
    {
        const string query =
            $"""
             SELECT *
             FROM stealthmetrics
             """;

        return await _queryProcessor.QueryAsync<StealthMetrics>(query);
    }

    public async Task<Result<StealthMetrics, Error>> GetByShadowIdAsync(Guid id)
    {
        const string query =
            $"""
             SELECT *
             FROM stealthmetrics
             WHERE shadow_id = @id
             """;

        return await _queryProcessor.QuerySingleOrDefaultAsync<StealthMetrics>(query,
            $"Stealth Metrics with ShadowId: {id} not found", new { id });
    }
}