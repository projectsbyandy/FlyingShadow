using System.Data;
using FlyingShadow.Core.Db;
using Npgsql;

namespace FlyingShadow.Api.Db;

internal class NpgSqlConnectionFactory : IDbConnectionFactory, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgSqlConnectionFactory(string connectionString)
    {
        _dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
    }

    public async Task<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return await _dataSource.OpenConnectionAsync(cancellationToken); 
    }
    
    public async ValueTask DisposeAsync()
    {
        await _dataSource.DisposeAsync();
    }
}