using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;

namespace FlyingShadow.Api.Repositories;

public interface IUserRepository
{
    public Result<User, Error> GetUser(string email);
    public Result<User, Error> AddUser(User user);
    public Result<Outcome, Error> EnsureUserDoesNotExist(string email);
}