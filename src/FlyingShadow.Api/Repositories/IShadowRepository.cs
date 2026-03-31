using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Repositories;

public interface IShadowRepository
{
    public Result<IList<Shadow>, Error> GetAll();
}