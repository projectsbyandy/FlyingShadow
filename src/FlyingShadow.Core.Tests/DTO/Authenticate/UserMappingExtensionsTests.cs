using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.Models.Users;

namespace FlyingShadow.Core.Tests.DTO.Authenticate;

public class UserMappingExtensionsTests
{
    [Fact]
    public void Verify_User_To_UserDto_Mapping()
    {
        // Arrange
        var sut = new User()
        {
            UserId = Guid.NewGuid(),
            Email = "tester@test.com",
            HashedPassword = "SomeHashedPassword",
        };
        
        // Act
        var userDto = sut.ToDto();
        
        // Assert
        Assert.Equal(sut.UserId, userDto.UserId);
        Assert.Equal(sut.Email, userDto.Email);
    }
}