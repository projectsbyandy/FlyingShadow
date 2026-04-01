using FlyingShadow.Core.Models.Users;

namespace FlyingShadow.Core.DTO.Authenticate;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user) => new UserDto
    {
        UserId = user.UserId,
        Email = user.Email
    };
}