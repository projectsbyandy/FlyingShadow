using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Repositories;

public interface IStealthMetricsRepository
{
    public Task<Result<IList<StealthMetrics>, Error>> GetAllAsync();
    public Task<Result<StealthMetrics, Error>> GetByShadowIdAsync(Guid id);
}