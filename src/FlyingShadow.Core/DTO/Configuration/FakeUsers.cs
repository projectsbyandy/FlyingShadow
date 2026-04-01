using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.Models.Users;

namespace FlyingShadow.Core.DTO.Configuration;

public class FakeUsers
{
    public IList<LoginDetails>? LoginDetailsList { get; init; } = default;
    public IList<User>? Users { get; init; } = default;

}