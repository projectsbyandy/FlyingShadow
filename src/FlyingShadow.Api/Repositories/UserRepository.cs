using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly IQueryProcessor _queryProcessor;

    public UserRepository(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    public async Task<Result<User, Error>> GetUserAsync(string email)
    {
        const string query =
            $"""
             SELECT * 
             FROM users 
             WHERE email = @email
             """;

        return await _queryProcessor.QuerySingleOrDefaultAsync<User>(query, $"User with email: {email} not found",
            new { email });
    }

    public async Task<Result<User, Error>> AddUserAsync(User user)
    {
        const string query =
            $"""
             INSERT INTO users(user_id, email, hashed_password) 
             VALUES(@userid::uuid, @email, @hashedPassword)
             """;

        var result = await _queryProcessor.ExecuteAsync(query, new { user.UserId, user.Email, user.HashedPassword });

        return result.IsSuccess
            ? result.Value is 1
                ? Result<User, Error>.Success(user)
                : Result<User, Error>.Failure(new Error(ErrorCode.UnexpectedError, $"Only expecting 1 row updated but got {result.Value}"))
            : Result<User, Error>.Failure(result.Error);
    }

    public async Task<Result<Outcome, Error>> EnsureUserDoesNotExistAsync(string email)
    {
        var result = await GetUserAsync(email);

        return result is { IsFailure: true, Error.Code: ErrorCode.NotFound }
            ? Result<Outcome, Error>.Success(Outcome.Value)
            : Result<Outcome, Error>.Failure(new Error(ErrorCode.AlreadyExists,
                $"User with {email} already registered"));
    }
}