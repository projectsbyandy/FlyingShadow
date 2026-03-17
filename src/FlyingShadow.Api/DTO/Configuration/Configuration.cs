using FlyingShadow.Api.DTO.Authenticate;

namespace FlyingShadow.Api.DTO.Configuration;

internal record Configuration
{
    public Jwt? Jwt { get; init; }
    public IList<User>? ValidFakeUsers { get; init; } = default;
}