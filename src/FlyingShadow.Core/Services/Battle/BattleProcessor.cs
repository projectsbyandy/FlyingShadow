using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Battle;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services.Battle;

public class BattleProcessor : IBattleProcessor
{
    public Result<BattleResponse, Error> Process(ShadowDto shadowOne, ShadowDto shadowTwo)
    {
        var validationResult = ValidateShadows(shadowOne, shadowTwo);
        if (validationResult.IsFailure)
            return Result<BattleResponse, Error>.Failure(validationResult.Error);
        
        var shadowOneStats = BattleStatsCalculator.Process(shadowOne);
        var shadowTwoStats = BattleStatsCalculator.Process(shadowTwo);
        
        return Result<BattleResponse, Error>.Success(BuildBattleReport(shadowOneStats, shadowTwoStats).ToBattleResponse());
    }
    
    private Result<Outcome, Error> ValidateShadows(ShadowDto shadowOne, ShadowDto shadowTwo)
    {
        if (shadowOne.Id.Equals(shadowTwo.Id))
            return Result<Outcome, Error>.Failure(new Error(ErrorCode.UnableToProcessData,
                "Shadows with the same Id cannot battle"));
        
        return shadowOne.Clan.Equals(shadowTwo.Clan) 
            ? Result<Outcome, Error>.Failure(new Error(ErrorCode.UnableToProcessData, "Shadows of the same clan cannot battle"))
            : Result<Outcome, Error>.Success(Outcome.Value);
    }
    
    private BattleReport BuildBattleReport(Stats shadowOneStats, Stats shadowTwoStats)
    {
        return new BattleReport()
        {
            Outcome = GetStatWinner(shadowOneStats.OverallRating, shadowTwoStats.OverallRating, shadowOneStats.CodeName, shadowTwoStats.CodeName),
            ShadowOneStats = shadowOneStats,
            ShadowTwoStats = shadowTwoStats,
            StatBreakdown = new StatResults()
            {
                CombatPowerWinner = GetStatWinner(shadowOneStats.CombatPower, shadowTwoStats.CombatPower, shadowOneStats.CodeName, shadowTwoStats.CodeName),
                EvasionIndexWinner = GetStatWinner(shadowOneStats.EvasionIndex, shadowTwoStats.EvasionIndex, shadowOneStats.CodeName, shadowTwoStats.CodeName),
                StealthScoreWinner = GetStatWinner(shadowOneStats.StealthScore,shadowTwoStats.StealthScore, shadowOneStats.CodeName, shadowTwoStats.CodeName)
            }
        };
    }

    private static string GetStatWinner(decimal statOne, decimal statTwo, string shadowOneName, string shadowTwoName)
    {
        return statOne > statTwo 
            ? shadowOneName 
            : statTwo > statOne 
                ? shadowTwoName 
                : "Draw";
    }
}