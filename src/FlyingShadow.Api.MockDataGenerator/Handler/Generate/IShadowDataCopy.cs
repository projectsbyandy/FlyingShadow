using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate;

internal interface IShadowDataCopy
{
    public Result<int, int> Process(PipelineContext context);
}