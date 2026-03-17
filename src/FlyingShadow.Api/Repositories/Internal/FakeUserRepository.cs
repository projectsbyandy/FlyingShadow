using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.ResultType;
using FlyingShadow.Api.Utils;

namespace FlyingShadow.Api.Repositories.Internal;

internal class FakeUserRepository : IUserRepository
{
    private IList<StoredUser>? _storedUsers;
    
    public FakeUserRepository()
    {
        SeededUsersAsync().Wait();
    }
    
    public Result<StoredUser> GetUser(string email)
    {
        var user = _storedUsers?.SingleOrDefault(user => user.Email.Equals(email));
        return user is not null 
            ? Result<StoredUser>.Success(user)
            : Result<StoredUser>.Failure(new Error("NOT_FOUND", $"User with {email} was not found"));
    }

    public Result<Guid> AddUser(User user)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, workFactor:14);
        var userToCreate = new StoredUser()
        {
            Email = user.Email,
            Hash = passwordHash
        };
        _storedUsers?.Add(userToCreate);

        return Result<Guid>.Success(userToCreate.UserId);
    }
    
    private async Task SeededUsersAsync()
    {
        _storedUsers = await FileReader.ReadAsync<IList<StoredUser>>("FakeUsers.json");
    }
}