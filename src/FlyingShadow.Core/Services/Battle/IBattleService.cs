using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services.Battle;

public interface IBattleService
{
    public Result<BattleResponse, Error> Battle(string shadowOneName, string shadowTwoName);
}