using System.Text.Json.Serialization;

namespace FlyingShadow.Core.Models.Users;

public record User
{
    public User() { } // For Dapper
    public Guid UserId { get; init; } = Guid.NewGuid();
    
    public required string Email { get; init; }

    [JsonIgnore]
    public string? HashedPassword { get; init; }
}