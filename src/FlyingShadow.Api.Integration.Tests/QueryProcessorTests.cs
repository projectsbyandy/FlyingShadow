using FlyingShadow.Api.Db;
using FlyingShadow.Api.Integration.Tests.Fixtures;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Integration.Tests;

public class QueryProcessorTests : IClassFixture<PgSqlTestContainerFixture>, IAsyncLifetime
{
    private IQueryProcessor? _sut;
    private readonly PgSqlTestContainerFixture _dbFixture;
    private readonly IDbConnectionFactory _dbFactory;
    
    public ValueTask InitializeAsync() => _dbFixture.ResetAsync();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    
    public QueryProcessorTests(PgSqlTestContainerFixture dbFixture)
    {
        _dbFixture = dbFixture;
        _dbFactory = new NpgSqlConnectionFactory(_dbFixture.ConnectionString);
    }

    [Fact]
    public async Task QueryAsync_WithData_ReturnsSuccessResultWithShadows()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        
        // Act
        var result = await _sut.QueryAsync<Shadow>("SELECT * FROM shadows");
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count());
    }
    
    [Fact]
    public async Task QuerySingleOrDefaultAsync_WithValidCodeName_ReturnsSuccessResultWithShadow()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        
        // Act
        var result = await _sut.QuerySingleOrDefaultAsync<Shadow>("SELECT * FROM shadows WHERE code_name = 'Silent Talon'");
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-000000000179"), result.Value.Id);
        Assert.Equal("Uzumaki Clan", result.Value.Clan);
        Assert.Equal("Silent Talon", result.Value.CodeName);
        Assert.Equal("Land of Sound", result.Value.Origin);
        Assert.Equal(Rank.Oniwaban, result.Value.Rank);
    }
    
    [Fact]
    public async Task QuerySingleOrDefaultAsync_WithData_ReturnsFailureWithNotFoundWithDefaultMessage()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        
        // Act
        var result = await _sut.QuerySingleOrDefaultAsync<Shadow>("SELECT * FROM shadows WHERE code_name = 'DoesNotExist'");
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, "Item not found"), result.Error);
    }
    
    [Fact]
    public async Task QuerySingleOrDefaultAsync_WithCustomNotFoundMessage_ReturnsFailureWithCustomMessage()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        const string customNotFoundMessage = "This Shadow does not exist";
        
        // Act
        var result = await _sut.QuerySingleOrDefaultAsync<Shadow>("SELECT * FROM shadows WHERE code_name = 'DoesNotExist'", customNotFoundMessage);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, customNotFoundMessage), result.Error);
    }
    
    [Fact]
    public async Task ExecuteAsync_WithExistingPrimaryKey_ReturnsFailureWithConflict()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        
        // Act
        var result = await _sut.ExecuteAsync("INSERT INTO shadows (id, code_name, clan, origin, rank) " +
                                             "VALUES ('550e8400-e29b-41d4-a716-000000000178', 'Silent Dagger', 'Hyuga Clan',  'Land of Rain', 'Toshiyama')");
       
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.Conflict, "duplicate key value violates unique constraint \"shadows_pkey\""), result.Error);
    }
    
    [Fact]
    public async Task ExecuteAsync_WithValidInsert_ReturnsSuccessWithUpdatedRows()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        
        // Act
        var result = await _sut.ExecuteAsync("INSERT INTO shadows (id, code_name, clan, origin, rank) " +
                                             "VALUES ('a117c09c-f761-4606-b17c-a7b828c2a22e', 'Silent Dagger', 'Hyuga Clan',  'Land of Rain', 'Toshiyama')");
       
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }
    
    [Fact]
    public async Task ExecuteAsync_WithInvalidSqlValue_ReturnsFailureUnableToProcess()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        
        // Act
        var result = await _sut.ExecuteAsync("INSERT INTO shadows (id, code_name, clan, origin, rank) " +
                                             "VALUES ('bob', 'Silent Dagger', 'Hyuga Clan',  'Land of Rain', 'Toshiyama')");
       
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.UnableToProcessData, "invalid input syntax for type uuid: \"bob\""), result.Error);
    }
    
    [Fact]
    public async Task ExecuteAsync_WithNonExistentForeignKey_ReturnsFailureWithForeignKeyConstraintConflict()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        
        // Act
        var result = await _sut.ExecuteAsync("INSERT INTO stealthmetrics (shadow_id, shadow_blend_score, silence_rating, invisibility_duration_ms, acrobatics_level) " +
                                             "VALUES ('2855b7f6-24ff-43bf-b30c-b6538dea7a54', 98, 48, 4215, 'Intermediate')");
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.Conflict, "insert or update on table \"stealthmetrics\" violates foreign key constraint \"stealthmetrics_shadow_id_fkey\""), result.Error);
    }
}