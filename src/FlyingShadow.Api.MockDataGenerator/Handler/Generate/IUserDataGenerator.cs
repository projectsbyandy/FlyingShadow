using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate;

internal interface IUserDataGenerator
{
    public Task<Result<PipelineContext, FailureCode>> CredentialsAsync(PipelineContext context);
    public Task<Result<PipelineContext, FailureCode>> WriteJwtFileAsync(PipelineContext context);
    public Task<Result<PipelineContext, FailureCode>> WriteLoginDetailsFileAsync(PipelineContext context);
    public Task<Result<PipelineContext, FailureCode>> WriteUsersFileAsync(PipelineContext context);
}