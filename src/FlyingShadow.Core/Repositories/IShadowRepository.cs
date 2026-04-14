using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Repositories;

public interface IShadowRepository
{
    public Result<IList<Shadow>, Error> GetAll();
    public Result<Shadow, Error> GetByCodeName(string codeName);
}