using Ardalis.GuardClauses;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace FlyingShadow.Api.Integration.Tests.Fixtures;

public class PgSqlTestContainerFixture : IAsyncLifetime
{
    private Respawner? _respawner;
    private NpgsqlConnection? _npgsqlConnection;
    
    private static string SchemaFile => Path.Combine(
        Directory.GetCurrentDirectory(),
        "./Support/DbDataSource/FlyingShadowTestContainerSchemaSetup.sql"
    );
    
    private static string SeedDataFile => Path.Combine(
        Directory.GetCurrentDirectory(),
        "./Support/DbDataSource/FlyingShadowTestContainerDataSeeder.sql"
    );
    
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17")
        .WithPortBinding(47777, 5432)
        .WithDatabase("flyingshadow")
        .WithUsername("tester")
        .WithPassword("pass123")
        .WithBindMount(SchemaFile, "/docker-entrypoint-initdb.d/init.sql")
        .Build();
    
    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        await _container.StartAsync();
        
        _npgsqlConnection = new NpgsqlConnection(_container.GetConnectionString());
        await _npgsqlConnection.OpenAsync();
        
        _respawner = await Respawner.CreateAsync(_npgsqlConnection, new RespawnerOptions {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }

    public async ValueTask ResetAsync()
    {
        Guard.Against.Null(_npgsqlConnection);
        Guard.Against.Null(_respawner);
        await _respawner.ResetAsync(_npgsqlConnection);
        await ReseedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_npgsqlConnection is not null)
            await _npgsqlConnection.DisposeAsync();
        
        await _container.DisposeAsync();
    }

    private async Task ReseedAsync()
    {
        Guard.Against.Null(_npgsqlConnection);
        var sql = await File.ReadAllTextAsync(SeedDataFile);
        await using var cmd = _npgsqlConnection.CreateCommand();
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync();
    }
}