using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate;

internal interface IShadowDataCopier
{
    public Result<SuccessCode, FailureCode> Process(PipelineContext context);
}