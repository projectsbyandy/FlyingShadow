using FlyingShadow.Api.DTO.Shadow;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Services;

public interface IShadowService
{
    public Result<IList<ShadowDto>, Error> GetAllShadowDetails();
}