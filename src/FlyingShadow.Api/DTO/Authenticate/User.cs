namespace FlyingShadow.Api.DTO.Authenticate;

public record User
{
    public Guid UserId { get; set; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}