using Ardalis.GuardClauses;
using Dapper;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<User, Error>> GetUserAsync(string email)
    {
        try
        {
            const string query = 
                $"""
                 SELECT * 
                 FROM users 
                 WHERE email = @email
                 """;
        
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var user = Guard.Against.Null(conn.QuerySingleOrDefault<User>(query, new { email }));
            
            return Result<User, Error>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, ex.Message));
        }    
    }

    public async Task<Result<User, Error>> AddUserAsync(User user)
    {
        try
        {
            const string query = 
                $"""
                 INSERT INTO users(user_id, email, hashed_password) 
                 VALUES(@userid::uuid, @email, @hashedPassword)
                 """;
        
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var rowsAffected = await conn.ExecuteAsync(query, new { user.UserId, user.Email, user.HashedPassword });
            
            return rowsAffected > 0
                ? Result<User, Error>.Success(user)
                : Result<User, Error>.Failure(new Error(ErrorCode.UnableToProcessData, $"Problem adding new user {user.Email}"));
        }
        catch (Exception ex)
        {
            return Result<User, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, ex.Message));
        }
    }

    public async Task<Result<Outcome, Error>> EnsureUserDoesNotExistAsync(string email)
    {
        try
        {
            const string query = 
                $"""
                 SELECT * 
                 FROM users 
                 WHERE email = @email
                 """;
        
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var user = conn.QuerySingleOrDefault<User>(query, new { email });

            return user is null
                ? Result<Outcome, Error>.Success(Outcome.Value)
                : Result<Outcome, Error>.Failure(new Error(ErrorCode.AlreadyExists,
                    $"User with {email} already registered"));
        }
        catch (Exception ex)
        {
            return Result<Outcome, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, ex.Message));
        }
    }
}