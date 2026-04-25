using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate;

internal interface IUserDataGenerator
{
    public Task<Result<PipelineContext, int>> CredentialsAsync(PipelineContext context);
    public Task<Result<PipelineContext, int>> WriteJwtFileAsync(PipelineContext context);
    public Task<Result<PipelineContext, int>> WriteLoginDetailsFileAsync(PipelineContext context);
    public Task<Result<PipelineContext, int>> WriteUsersFileAsync(PipelineContext context);
}