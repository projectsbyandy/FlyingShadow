using FlyingShadow.Api.Models.Ninja;
using FlyingShadow.Api.Models.ResultType;

namespace FlyingShadow.Api.Repositories;

public interface IStealthMetricsRepository
{
    public Result<IList<StealthMetrics>, Error> GetAll();
}