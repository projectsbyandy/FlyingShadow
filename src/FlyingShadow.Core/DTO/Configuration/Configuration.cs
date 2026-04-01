namespace FlyingShadow.Core.DTO.Configuration;

public record Configuration
{
    public Jwt? Jwt { get; init; }
}