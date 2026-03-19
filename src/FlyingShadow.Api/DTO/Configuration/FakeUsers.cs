using FlyingShadow.Api.DTO.Authenticate;

namespace FlyingShadow.Api.DTO.Configuration;

internal class FakeUsers
{
    public IList<User>? Users { get; init; } = default;
    public IList<DbUser>? DbUsers { get; init; } = default;

}