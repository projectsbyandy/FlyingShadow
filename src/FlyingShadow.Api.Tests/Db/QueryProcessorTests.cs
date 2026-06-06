using FlyingShadow.Api.Db;
using FlyingShadow.Core.Db;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using Moq;
using Npgsql;

namespace FlyingShadow.Api.Tests.Db;

public class QueryProcessorTests
{
    [Fact]
    public async Task ExecuteAsync_WithInvalidDb_ReturnsFailureWithDbConnectionProblem()
    {
        // Arrange
        var dbFactoryMock = new Mock<IDbConnectionFactory>();
        dbFactoryMock.Setup(factory => factory.OpenConnectionAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NpgsqlException("Password provider threw an exception"));

        var sut = new QueryProcessor(dbFactoryMock.Object);

        // Act
        var result = await sut.ExecuteAsync("INSERT INTO DoesNotExist (id, code_name, clan, origin, rank) " +
                                             "VALUES ('bob', 'Silent Dagger', 'Hyuga Clan',  'Land of Rain', 'Toshiyama')");

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new Error(ErrorCode.DbConnectionProblem, "Password provider threw an exception"),
            result.Error);
    }
}