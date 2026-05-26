using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;

namespace FlyingShadow.Core.Repositories;

public interface IUserRepository
{
    public Task<Result<User, Error>> GetUserAsync(string email);
    public Task<Result<User, Error>> AddUserAsync(User user);
    public Task<Result<Outcome, Error>> EnsureUserDoesNotExistAsync(string email);
}