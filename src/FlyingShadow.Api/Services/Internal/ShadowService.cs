using FlyingShadow.Api.DTO.Ninja;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Repositories;

namespace FlyingShadow.Api.Services.Internal;

public class ShadowService : IShadowService
{
    private readonly IShadowRepository  _shadowRepository;
    private readonly IStealthMetricsRepository _stealthMetricsRepository;

    public ShadowService(IShadowRepository shadowRepository, IStealthMetricsRepository stealthMetricsRepository)
    {
        _shadowRepository = shadowRepository;
        _stealthMetricsRepository = stealthMetricsRepository;
    }

    public Result<IList<ShadowDto>, Error> GetAllShadowDetails()
    {
        throw new NotImplementedException();
    }
}