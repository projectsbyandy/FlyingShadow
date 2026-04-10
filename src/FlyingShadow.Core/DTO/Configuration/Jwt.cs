namespace FlyingShadow.Core.DTO.Configuration;

public record Jwt
{
    public string? Key { get; init; }
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public double ExpiryInHours { get; init; }
}