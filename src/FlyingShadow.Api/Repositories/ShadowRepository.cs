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
            const string query = 
                $"""
                SELECT *
                FROM shadows
                """;
        
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var shadows = Guard.Against.Null(conn.Query<Shadow>(query)).ToList();
            
            return Result<IList<Shadow>, Error>.Success(shadows);
        }
        catch (Exception ex)
        {
            return Result<IList<Shadow>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, ex.Message));
        }
    }

    public async Task<Result<Shadow, Error>> GetByCodeNameAsync(string codeName)
    {
        try
        {
            const string query = 
                $"""
                 SELECT *
                 FROM shadows
                 WHERE code_name = @codeName
                 """;
        
            using var conn = await _dbConnectionFactory.OpenConnectionAsync();
            var shadow = conn.QuerySingleOrDefault<Shadow>(query, new {codeName});
            
            return shadow is null
                ? Result<Shadow, Error>.Failure(new Error(ErrorCode.NotFound, $"Shadow with CodeName: {codeName} not found"))
                : Result<Shadow, Error>.Success(shadow);
        }
        catch (Exception ex)
        {
            return Result<Shadow, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, ex.Message));
        }
    }
}