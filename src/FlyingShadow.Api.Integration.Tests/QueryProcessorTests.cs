using FlyingShadow.Api.Db;
using FlyingShadow.Api.Integration.Tests.Fixtures;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.Integration.Tests;

public class QueryProcessorTests : IClassFixture<PgSqlTestContainerFixture>, IAsyncLifetime
{
    private IQueryProcessor _sut;
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
        var result = await _sut.QueryAsync<Shadow>("Select * From Shadows");
        
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
        var result = await _sut.QuerySingleOrDefaultAsync<Shadow>("Select * From Shadows WHERE code_name = 'Silent Talon'");
        
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
    public async Task QuerySingleOrDefaultAsync_WithData_ReturnsFailureWithNotFound()
    {
        // Arrange
        _sut = new QueryProcessor(_dbFactory);
        
        // Act
        var result = await _sut.QuerySingleOrDefaultAsync<Shadow>("Select * From Shadows WHERE code_name = 'DoesNotExist'");
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.NotFound, "Item not found"), result.Error);
    }
}