using FlyingShadow.Api.Models.Ninja;
using FlyingShadow.Api.Models.ResultType;

namespace FlyingShadow.Api.Repositories;

public interface IShadowRepository
{
    public Result<IList<Shadow>, Error> GetAll();
}