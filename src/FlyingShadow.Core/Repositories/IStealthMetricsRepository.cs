using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Repositories;

public interface IStealthMetricsRepository
{
    public Result<IList<StealthMetrics>, Error> GetAll();
}