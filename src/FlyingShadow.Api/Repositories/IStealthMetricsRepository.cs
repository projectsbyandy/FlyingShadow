using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Repositories;

public interface IStealthMetricsRepository
{
    public Result<IList<StealthMetrics>, Error> GetAll();
}