using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Repositories;

public interface IShadowRepository
{
    public Task<Result<IEnumerable<Shadow>, Error>> GetAllAsync();
    public Task<Result<Shadow, Error>> GetByCodeNameAsync(string codeName);
}