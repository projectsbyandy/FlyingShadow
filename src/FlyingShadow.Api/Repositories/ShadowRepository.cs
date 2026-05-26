using Ardalis.GuardClauses;
using Dapper;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class ShadowRepository : IShadowRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ShadowRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<IList<Shadow>, Error>> GetAllAsync()
    {
        try
        {
            const string sql = $"""
                                SELECT *
                                FROM Shadows
                                """;
        
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var shadows = Guard.Against.Null(conn.QuerySingleOrDefault<IList<Shadow>>(sql));
            
            return Result<IList<Shadow>, Error>.Success(shadows);
        }
        catch (Exception ex)
        {
            return Result<IList<Shadow>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, ex.Message));
        }
    }

    public Task<Result<Shadow, Error>> GetByCodeNameAsync(string codeName)
    {
        throw new NotImplementedException();
    }
}