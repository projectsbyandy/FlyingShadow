namespace FlyingShadow.Core.DTO.Configuration;

public record Configuration
{
    public Jwt? Jwt { get; init; }
    public DbServer? DbServer { get; init; }
}