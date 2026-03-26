using Ardalis.GuardClauses;
using FlyingShadow.Api.Models.Ninja;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Utils;

namespace FlyingShadow.Api.Repositories.Internal;

internal class FakeStealthMetricsRepository : WithMockData<IList<StealthMetrics>>, IStealthMetricsRepository
{
    private static IList<StealthMetrics>? _stealthMetrics;

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
        return _stealthMetrics is null
            ? Result<IList<StealthMetrics>, Error>.Failure(new Error("UNABLE_TO_LOAD_STEALTH_METRICS", "Unable to fetch stealth metrics."))
            : Result<IList<StealthMetrics>, Error>.Success(_stealthMetrics);
    }
}