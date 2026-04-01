namespace FlyingShadow.Core.DTO.Authenticate;

public record RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}