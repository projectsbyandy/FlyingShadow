using Ardalis.GuardClauses;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Configuration;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class FakeUserRepository : WithMockData<IList<User>>, IUserRepository
{
    private readonly IList<User> _users;
    
    public FakeUserRepository()
    {
        _users = LoadMockData();
    }

    public sealed override IList<User> LoadMockData()
    {
        return Guard.Against.Null(ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers").Users, 
            "Users Mock data has not been configured");
    }

    public Result<User, Error> GetUser(string email)
    {
        var user = _users.SingleOrDefault(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return user is not null 
            ? Result<User, Error>.Success(user)
            : Result<User, Error>.Failure(new Error("NOT_FOUND", $"User with {email} was not found"));
    }

    public Result<User, Error> AddUser(User user)
    {
        _users.Add(user);

        return Result<User, Error>.Success(user);
    }

    public Result<Outcome, Error> EnsureUserDoesNotExist(string email)
    {
        var exists = _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        
        return exists
            ? Result<Outcome, Error>.Failure(new Error("ALREADY_REGISTERED", $"User with {email} already registered"))
            : Result<Outcome, Error>.Success(Outcome.Value);
    }
}