using FlyingShadow.Api.DTO.Shadow;
using FlyingShadow.Api.Models.Ninja;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Repositories;

namespace FlyingShadow.Api.Services.Internal;

public class ShadowService : IShadowService
{
    private readonly IShadowRepository _shadowRepository;
    private readonly IStealthMetricsRepository _stealthMetricsRepository;

    public ShadowService(IShadowRepository shadowRepository, IStealthMetricsRepository stealthMetricsRepository)
    {
        _shadowRepository = shadowRepository;
        _stealthMetricsRepository = stealthMetricsRepository;
    }

    public Result<IList<ShadowDto>, Error> GetAllShadowDetails()
    {
        var shadows = _shadowRepository.GetAll().Value;
        var metrics = _stealthMetricsRepository.GetAll().Value;

        if (shadows is null || metrics is null)
            return Result<IList<ShadowDto>, Error>.Failure(new Error("NO_SHADOW_OR_METRIC_DATA", "No Shadow or Metric Data Retrieved"));

        var metricsById = metrics.ToDictionary(m => m.ShadowId);

        var shadowDtos = shadows
            .Where(s => metricsById.ContainsKey(s.Id))
            .Select(s => MapToDto(s, metricsById[s.Id]))
            .ToList();

        return shadowDtos.Count > 0
            ? Result<IList<ShadowDto>, Error>.Success(shadowDtos)
            : Result<IList<ShadowDto>, Error>.Failure(new Error("NO_SHADOW_DETAILS_MAPPED", "No Shadow Details mapped"));
    }
    
    private static ShadowDto MapToDto(Shadow s, StealthMetrics m) => new()
    {
        Id       =  s.Id,
        Clan     = s.Clan,
        CodeName = s.CodeName,
        Origin   = s.Origin,
        Rank     = s.Rank,
        ShadowSkills = new()
        {
            AcrobaticsLevel        = m.AcrobaticsLevel,
            InvisibilityDurationMs = m.InvisibilityDurationMs,
            ShadowBlendScore       = m.ShadowBlendScore,
            SilenceRating          = m.SilenceRating,
        }
    };
}