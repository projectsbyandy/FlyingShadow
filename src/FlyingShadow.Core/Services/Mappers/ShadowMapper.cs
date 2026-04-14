using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Services.Mappers;

public class ShadowMapper : IShadowMapper
{
    public ShadowDto ToDto(Shadow shadow, StealthMetrics metrics) => new()
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