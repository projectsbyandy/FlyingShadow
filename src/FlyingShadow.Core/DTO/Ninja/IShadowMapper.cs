using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.DTO.Ninja;

public interface IShadowMapper
{
    ShadowDto ToDto(Shadow shadow, StealthMetrics stealthMetrics);
}