namespace FlyingShadow.Api.DTO.Configuration;

internal record Jwt
{
    public required string Secret { get; set; }
}