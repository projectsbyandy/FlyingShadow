namespace FlyingShadow.Api.DTO.Configuration;

internal record Configuration
{
    public Jwt? Jwt { get; init; }
    public FakeUsers? FakeUsers { get; set; }
}