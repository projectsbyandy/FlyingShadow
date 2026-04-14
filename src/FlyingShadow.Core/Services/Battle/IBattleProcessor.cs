using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services.Battle;

public interface IBattleProcessor
{
    public Result<BattleResponse, Error> Process(ShadowDto shadowOne, ShadowDto shadowTwo);
}