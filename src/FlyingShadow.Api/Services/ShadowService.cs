using Ardalis.GuardClauses;
using FlyingShadow.Core.DTO.Shadow;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;

namespace FlyingShadow.Api.Services;

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
        try
        {
            var shadowResult = _shadowRepository.GetAll();
            var stealthMetricsResult = _stealthMetricsRepository.GetAll();

            if (!shadowResult.IsSuccess || !stealthMetricsResult.IsSuccess)
                return Result<IList<ShadowDto>, Error>.Failure(new Error("NO_SHADOW_OR_METRIC_DATA",
                    "Unable to retrieve Shadow or Metric Data"));

            var metricsById = Guard.Against.Null(stealthMetricsResult.Value).ToDictionary(m => m.ShadowId);

            var shadowDtos = Guard.Against.Null(shadowResult.Value)
                .Where(s => metricsById.ContainsKey(s.Id))
                .Select(s => MapToDto(s, metricsById[s.Id]))
                .ToList();

            return shadowDtos.Count > 0
                ? Result<IList<ShadowDto>, Error>.Success(shadowDtos)
                : Result<IList<ShadowDto>, Error>.Failure(new Error("NO_SHADOW_DETAILS_MAPPED",
                    "No Shadow Details mapped"));
        }
        catch (Exception ex)
        {
            return Result<IList<ShadowDto>, Error>.Failure(new Error("UNEXPECTED_ERROR", ex.Message));
        }
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