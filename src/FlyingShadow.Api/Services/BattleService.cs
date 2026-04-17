using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services.Battle;
using FlyingShadow.Core.Services.Mappers;

namespace FlyingShadow.Api.Services;

internal class BattleService : IBattleService
{
    private readonly IShadowRepository _shadowRepository;
    private readonly IShadowDtoMapper _shadowDtoMapper;
    private readonly IStealthMetricsRepository _stealthMetricsRepository;
    private readonly IBattleProcessor _battleProcessor;

    public BattleService(IShadowRepository shadowRepository, IShadowDtoMapper shadowDtoMapper, IStealthMetricsRepository stealthMetricsRepository, IBattleProcessor battleProcessor)
    {
        _shadowRepository = shadowRepository;
        _shadowDtoMapper = shadowDtoMapper;
        _stealthMetricsRepository = stealthMetricsRepository;
        _battleProcessor = battleProcessor;
    }

    public Result<BattleResponse, Error> Battle(string shadowOneName, string shadowTwoName)
    {
        var shadowOneResult = GetShadowDto(shadowOneName);
        var shadowTwoResult = GetShadowDto(shadowTwoName);

        if (shadowOneResult.IsFailure)
            return Result<BattleResponse, Error>.Failure(shadowOneResult.Error);
        
        if (shadowTwoResult.IsFailure)
            return Result<BattleResponse, Error>.Failure(shadowTwoResult.Error);
        
        return _battleProcessor.Process(shadowOneResult.Value, shadowTwoResult.Value);
    }

    private Result<ShadowDto, Error> GetShadowDto(string codeName)
    {
        return _shadowRepository.GetByCodeName(codeName)
            .Bind(shadow => _stealthMetricsRepository
                .GetByShadowId(shadow.Id)
                .Map(metrics => _shadowDtoMapper.ToSingle(shadow, metrics)));
    }
}