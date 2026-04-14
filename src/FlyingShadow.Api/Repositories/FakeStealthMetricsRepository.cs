using Ardalis.GuardClauses;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class FakeStealthMetricsRepository : WithMockData<IList<StealthMetrics>>, IStealthMetricsRepository
{
    private readonly IList<StealthMetrics> _stealthMetrics;

    public FakeStealthMetricsRepository()
    {
        _stealthMetrics = LoadMockData();
    }
    
    public sealed override IList<StealthMetrics> LoadMockData()
    {
        return Guard.Against.Null(ConfigReader.GetConfigurationSection<List<StealthMetrics>>("FakeStealthMetrics"), 
            "Stealth Metrics Mock data has not been configured");
    }

    public Result<IList<StealthMetrics>, Error> GetAll()
    {
        return Result<IList<StealthMetrics>, Error>.Success(_stealthMetrics);
    }

    public Result<StealthMetrics, Error> GetByShadowId(Guid id)
    {
        var metrics = _stealthMetrics.ToList()
            .Find(s => s.ShadowId.Equals(id));

        return metrics is null
            ? Result<StealthMetrics, Error>.Failure(new Error(ErrorCode.NotFound, $"Stealth metrics with ShadowId: {id} does not exist"))
            : Result<StealthMetrics, Error>.Success(metrics);
    }
}