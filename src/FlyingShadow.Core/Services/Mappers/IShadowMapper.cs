using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Core.Services.Mappers;

public interface IShadowMapper
{
    ShadowDto ToDto(Shadow shadow, StealthMetrics stealthMetrics);
}