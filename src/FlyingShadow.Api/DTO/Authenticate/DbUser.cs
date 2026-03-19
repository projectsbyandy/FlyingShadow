using System.Text.Json.Serialization;

namespace FlyingShadow.Api.DTO.Authenticate;

public record DbUser
{
    public Guid UserId { get; init; } = Guid.NewGuid();
    
    public required string Email { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string Hash { get; init; }
}