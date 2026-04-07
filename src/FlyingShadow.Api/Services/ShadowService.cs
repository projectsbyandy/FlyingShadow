using Ardalis.GuardClauses;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;

namespace FlyingShadow.Api.Services;

internal class ShadowService : IShadowService
{
    private readonly IShadowRepository _shadowRepository;
    private readonly IStealthMetricsRepository _stealthMetricsRepository;
    private readonly IShadowMapper _shadowMapper;

    public ShadowService(IShadowRepository shadowRepository, IStealthMetricsRepository stealthMetricsRepository, IShadowMapper shadowMapper)
    {
        _shadowRepository = shadowRepository;
        _stealthMetricsRepository = stealthMetricsRepository;
        _shadowMapper = shadowMapper;
    }

    public Result<IList<ShadowDto>, Error> GetAllShadowDetails()
    {
        try
        {
            var shadowResult = _shadowRepository.GetAll();
            var stealthMetricsResult = _stealthMetricsRepository.GetAll();

            if (!shadowResult.IsSuccess || !stealthMetricsResult.IsSuccess)
                return Result<IList<ShadowDto>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData,
                    "Unable to retrieve Shadow or Metric Data"));

            var metricsById = Guard.Against.Null(stealthMetricsResult.Value).ToDictionary(m => m.ShadowId);

            var shadowDtos = Guard.Against.Null(shadowResult.Value)
                .Where(s => metricsById.ContainsKey(s.Id))
                .Select(s => _shadowMapper.ToDto(s, metricsById[s.Id]))
                .ToList();

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