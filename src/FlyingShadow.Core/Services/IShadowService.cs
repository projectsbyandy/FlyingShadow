using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Services;

public interface IShadowService
{
    public Task<Result<IList<ShadowDto>, Error>> GetAllShadowDetailsAsync();
}