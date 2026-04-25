using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal interface IPreReqValidator
{
    public Result<FakeDataDestinationPaths, int> CheckArguments(string[] args);
    public Task<Result<PipelineContext, int>> CheckFilesExistAsync(FakeDataDestinationPaths destinationPaths);
}