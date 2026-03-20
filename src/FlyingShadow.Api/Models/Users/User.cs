using System.Text.Json.Serialization;

namespace FlyingShadow.Api.Models.Users;

public record User
{
    public Guid UserId { get; init; } = Guid.NewGuid();
    
    public required string Email { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string HashedPassword { get; init; }
}