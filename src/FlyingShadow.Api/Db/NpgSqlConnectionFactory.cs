using System.Data;
using Ardalis.GuardClauses;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.DTO.Configuration;
using Npgsql;

namespace FlyingShadow.Api.Db;

internal class NpgSqlConnectionFactory : IDbConnectionFactory, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgSqlConnectionFactory(Configuration configuration)
    {
        _dataSource = new NpgsqlDataSourceBuilder(BuildConnectionString(Guard.Against.Null(configuration.DbServer))).Build();
    }

    public async Task<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return await _dataSource.OpenConnectionAsync(cancellationToken); 
    }
    
    public async ValueTask DisposeAsync()
    {
        await _dataSource.DisposeAsync();
    }

    private string BuildConnectionString(DbServer config)
    {
        return
            $"Host={config.Host};Port={config.Port};Database={config.Database};" +
            $"Username={config.Username};Password={config.Password};" +
            $"Maximum Pool Size={config.MaxPoolSize};Minimum Pool Size={config.MinPoolSize}";
    }
}