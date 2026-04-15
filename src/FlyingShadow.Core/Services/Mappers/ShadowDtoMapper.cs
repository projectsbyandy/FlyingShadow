using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Services.Mappers;

public class ShadowDtoMapper : IShadowDtoMapper
{
    public List<ShadowDto> ToList(IList<Shadow> shadows, IList<StealthMetrics> stealthMetrics)
    {
        var metricsById = stealthMetrics.ToDictionary(m => m.ShadowId);

        return shadows
            .Where(s => metricsById.ContainsKey(s.Id))
            .Select(s => ToSingle(s, metricsById[s.Id]))
            .ToList();
    }
    
    public ShadowDto ToSingle(Shadow shadow, StealthMetrics metrics) => new()
    {
        Id = shadow.Id,
        Clan = shadow.Clan,
        CodeName = shadow.CodeName,
        Origin = shadow.Origin,
        Rank = shadow.Rank,
        ShadowSkills = new ShadowDto.StealthMetricsDto()
        {
            AcrobaticsLevel = metrics.AcrobaticsLevel,
            InvisibilityDurationMs = metrics.InvisibilityDurationMs,
            SilenceRating = metrics.SilenceRating,
            ShadowBlendScore = metrics.ShadowBlendScore
        }
    };
}