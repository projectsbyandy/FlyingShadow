namespace FlyingShadow.Api.DTO.Authenticate;

public record UserDto
{
    public Guid UserId { get; init; }
    public required string Email { get; init; }
}