using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Services.Mappers;

public class ShadowDtoMapper : IShadowDtoMapper
{
    private readonly IShadowMapper  _shadowMapper;

    public ShadowDtoMapper(IShadowMapper shadowMapper)
    {
        _shadowMapper = shadowMapper;
    }

    public List<ShadowDto> List(IList<Shadow> shadows, IList<StealthMetrics> stealthMetricsResult)
    {
        var metricsById = stealthMetricsResult.ToDictionary(m => m.ShadowId);

        return shadows
            .Where(s => metricsById.ContainsKey(s.Id))
            .Select(s => _shadowMapper.ToDto(s, metricsById[s.Id]))
            .ToList();
    }
    
    public ShadowDto Single(Shadow shadow, StealthMetrics stealthMetrics)
        => _shadowMapper.ToDto(shadow, stealthMetrics);
}