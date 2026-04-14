using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Services.Mappers;

public interface IShadowDtoMapper
{
    public List<ShadowDto> List(IList<Shadow> shadows, IList<StealthMetrics> stealthMetricsResult);
    public ShadowDto Single(Shadow shadow, StealthMetrics stealthMetrics);

}