using FlyingShadow.Api.DTO.Ninja;
using FlyingShadow.Api.Models.ResultType;

namespace FlyingShadow.Api.Services;

public interface IShadowService
{
    public Result<IList<ShadowDto>, Error> GetAllShadowDetails();
}