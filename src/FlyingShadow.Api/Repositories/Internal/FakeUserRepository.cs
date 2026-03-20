using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.DTO.ResultType;

namespace FlyingShadow.Api.Repositories.Internal;

internal class FakeUserRepository : IUserRepository
{
    private readonly Configuration _configuration;
    private IList<DbUser>? _dbUsers;
    
    public FakeUserRepository(Configuration configuration)
    {
        _configuration = configuration;
        AttachMockDbUsers();
    }

    private void AttachMockDbUsers()
    {
        if (_configuration.FakeUsers is null)
            Console.WriteLine("Mock data has not been configured");
        
        _dbUsers = _configuration.FakeUsers?.DbUsers;

    }

    public Result<DbUser, Error> GetUser(string email)
    {
        var user = _dbUsers?.SingleOrDefault(user => user.Email.Equals(email));
        return user is not null 
            ? Result<DbUser, Error>.Success(user)
            : Result<DbUser, Error>.Failure(new Error("NOT_FOUND", $"User with {email} was not found"));
    }

    public Result<Guid, Error> AddUser(User user)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, workFactor:14);
        var userToCreate = new DbUser()
        {
            Email = user.Email,
            Hash = passwordHash
        };
        _dbUsers?.Add(userToCreate);

        return Result<Guid, Error>.Success(userToCreate.UserId);
    }
}