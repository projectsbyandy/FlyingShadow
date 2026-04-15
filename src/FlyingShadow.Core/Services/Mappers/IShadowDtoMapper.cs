using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Services.Mappers;

public interface IShadowDtoMapper
{
    public List<ShadowDto> ToList(IList<Shadow> shadows, IList<StealthMetrics> stealthMetrics);
    public ShadowDto ToSingle(Shadow shadow, StealthMetrics metrics);

}