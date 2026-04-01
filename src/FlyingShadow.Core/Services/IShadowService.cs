using FlyingShadow.Core.DTO.Shadow;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services;

public interface IShadowService
{
    public Result<IList<ShadowDto>, Error> GetAllShadowDetails();
}