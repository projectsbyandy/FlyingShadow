using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services.Battle;

public interface IBattleService
{
    public Task<Result<BattleResponse, Error>> BattleAsync(string shadowOneName, string shadowTwoName);
}