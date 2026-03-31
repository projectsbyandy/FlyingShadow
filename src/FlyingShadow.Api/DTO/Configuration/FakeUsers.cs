using FlyingShadow.Api.DTO.Authenticate;
using FlyingShadow.Core.Models.Users;

namespace FlyingShadow.Api.DTO.Configuration;

internal class FakeUsers
{
    public IList<LoginDetails>? LoginDetailsList { get; init; } = default;
    public IList<User>? Users { get; init; } = default;

}