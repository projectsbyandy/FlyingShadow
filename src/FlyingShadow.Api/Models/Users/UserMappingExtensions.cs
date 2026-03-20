using FlyingShadow.Api.DTO.Authenticate;

namespace FlyingShadow.Api.Models.Users;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user) => new UserDto
    {
        UserId = user.UserId,
        Email = user.Email
    };
}