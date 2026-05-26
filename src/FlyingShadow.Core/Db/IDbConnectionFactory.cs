using System.Data;

namespace FlyingShadow.Core.Db;

public interface IDbConnectionFactory
{
    Task<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}