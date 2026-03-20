using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Models.Users;

namespace FlyingShadow.Api.Repositories.Internal;

internal class FakeUserRepository : IUserRepository
{
    private readonly Configuration _configuration;
    private IList<User> _users = [];
    
    public FakeUserRepository(Configuration configuration)
    {
        _configuration = configuration;
        AttachMockDbUsers();
    }

    private void AttachMockDbUsers()
    {
        if (_configuration.FakeUsers is null)
            Console.WriteLine("Mock data has not been configured");
        
        _users = _configuration.FakeUsers?.Users!;

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