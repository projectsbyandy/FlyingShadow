using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.ResultType;
using FlyingShadow.Api.Utils;

namespace FlyingShadow.Api.Repositories;

public interface IUserRepository
{
    public Result<StoredUser> GetUser(string name);
    public Result<Guid> AddUser(User user);
}