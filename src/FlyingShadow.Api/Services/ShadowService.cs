using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;
using FlyingShadow.Core.Services.Mappers;

namespace FlyingShadow.Api.Services;

internal class ShadowService : IShadowService
{
    private readonly IShadowRepository _shadowRepository;
    private readonly IStealthMetricsRepository _stealthMetricsRepository;
    private readonly IShadowDtoMapper _shadowDtoMapper;

    public ShadowService(IShadowRepository shadowRepository, IStealthMetricsRepository stealthMetricsRepository, IShadowDtoMapper shadowDtoMapper)
    {
        _shadowRepository = shadowRepository;
        _stealthMetricsRepository = stealthMetricsRepository;
        _shadowDtoMapper = shadowDtoMapper;
    }

    public Result<IList<ShadowDto>, Error> GetAllShadowDetails()
    {
        try
        {
            var shadowResult = _shadowRepository.GetAll();
            var stealthMetricsResult = _stealthMetricsRepository.GetAll();
             
            if (shadowResult.IsFailure || stealthMetricsResult.IsFailure)
                return Result<IList<ShadowDto>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, "Unable to retrieve Shadow or Metric Data"));
            
            var shadowDtos = _shadowDtoMapper.List(shadowResult.Value, stealthMetricsResult.Value);

            return shadowDtos.Count > 0
                ? Result<IList<ShadowDto>, Error>.Success(shadowDtos)
                : Result<IList<ShadowDto>, Error>.Failure(new Error(ErrorCode.UnableToProcessData,
                    "No Shadow Details mapped"));
        }
        catch (Exception ex)
        {
            return Result<IList<ShadowDto>, Error>.Failure(new Error(ErrorCode.UnexpectedError, ex.Message));
        }
    }
}