using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Api.DTO.Configuration;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Repositories.Internal;
using FlyingShadow.Api.Utils;
using Xunit;
using Assert = Xunit.Assert;

namespace FlyingShadow.Api.Tests.Repositories;

public class FakeUserRepositoryTests
{
    private static readonly Configuration Config = ConfigReader.GetConfiguration<Configuration>();
    private readonly IUserRepository _sut;
    
    public FakeUserRepositoryTests()
    {
        _sut = new FakeUserRepository(Config);    
    }

    [Fact]
    public void Verify_A_User_Can_be_Added()
    {
        // Arrange
        var user = new User()
        {
            Email = "andy@test.com",
            Password = "password2"
        };
          
        // Act
        var result = _sut.AddUser(user);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Value);
    }
    
    [Fact]
    public void Verify_()
    {
        _sut.AddUser(new()
        {
            Email = "andy@test.com",
            Password = "password2"
        });
    }
    
}