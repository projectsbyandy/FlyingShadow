using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;

internal class ShadowDataCopy : IShadowDataCopy
{
    public Result<int, int> Process(PipelineContext context)
    {
        try
        {
            File.Copy(Path.Combine(AppContext.BaseDirectory, "./StaticData/shadows.json"), context.FakeDataDestinationPaths.ShadowsPath, true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "./StaticData/stealthMetrics.json"), context.FakeDataDestinationPaths.StealthMetricsPath, true);
            Console.WriteLine($"MockDataGenerator: Copied Mock Shadow Data files");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MockDataGenerator: Problem copying Shadow Data {ex.Message}");
        
            return Result<int, int>.Failure(1);
        }
        
        return Result<int, int>.Success(0);
    }
}