using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;

internal class ShadowDataCopier : IShadowDataCopier
{
    public Result<SuccessCode, FailureCode> Process(PipelineContext context)
    {
        try
        {
            File.Copy(context.StaticShadowData, context.MockDataOptions.FakeShadowsPath, true);
            File.Copy(context.StaticStealthMetricsData, context.MockDataOptions.FakeStealthMetricsPath, true);
            Console.WriteLine($"MockDataGenerator: Copied Mock Shadow Data files");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MockDataGenerator: Problem copying Shadow Data {ex.Message}");
        
            return Result<SuccessCode, FailureCode>.Failure(FailureCode.Problem);
        }
        
        return Result<SuccessCode, FailureCode>.Success(SuccessCode.Ok);
    }
}