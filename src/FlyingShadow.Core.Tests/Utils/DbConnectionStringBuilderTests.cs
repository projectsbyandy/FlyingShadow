using System.Data.Common;
using FlyingShadow.Core.DTO.Configuration;
using DbConnectionStringBuilder = FlyingShadow.Core.Utils.DbConnectionStringBuilder;

namespace FlyingShadow.Core.Tests.Utils;

public class DbConnectionStringBuilderTests
{
    [Fact]
    public void Map_WithValidDbConfig_ReturnsConnectionString()
    {
        // Arrange
        var dbConfig = new DbServer()
        {
            Host =  "TestHost",
            Port = 7777,
            Database = "TestDatabase",
            Username = "TestUser",
            Password = "TestPassword",
            MinPoolSize = 1,
            MaxPoolSize = 2,
        };
        
        // Act
        var connectionString = DbConnectionStringBuilder.Map(dbConfig);
        
        // Assert
        Assert.Equal($"Host=TestHost;Port=7777;Database=TestDatabase;Username=TestUser;Password=TestPassword;Maximum Pool Size=2;Minimum Pool Size=1", connectionString);
    }
}