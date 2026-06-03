using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class ShadowRepository : IShadowRepository
{
    private readonly IQueryProcessor _queryProcessor;

    public ShadowRepository(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    public async Task<Result<IEnumerable<Shadow>, Error>> GetAllAsync()
    {
        const string query = 
            $"""
            SELECT *
            FROM shadows
            """;
        
        return await _queryProcessor.QueryAsync<Shadow>(query);
    }

    public async Task<Result<Shadow, Error>> GetByCodeNameAsync(string codeName)
    {
        const string query =
            $"""
             SELECT *
             FROM shadows
             WHERE code_name = @codeName
             """;

        return await _queryProcessor.QuerySingleOrDefaultAsync<Shadow>(query, $"Shadow with CodeName: {codeName} not found",new {codeName});
    }
}