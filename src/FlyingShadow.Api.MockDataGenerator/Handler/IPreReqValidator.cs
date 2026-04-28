using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal interface IPreReqValidator
{
    public Task<Result<PipelineContext, FailureCode>> CheckFilesExistAsync();
}