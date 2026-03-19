using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.ResultType;

namespace FlyingShadow.Api.Repositories;

public interface IUserRepository
{
    public Result<DbUser, Error> GetUser(string name);
    public Result<Guid, Error> AddUser(User user);
}