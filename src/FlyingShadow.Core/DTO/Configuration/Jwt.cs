namespace FlyingShadow.Core.DTO.Configuration;

public record Jwt
{
    public string? Key { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}