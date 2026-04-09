using System.Text.Json.Serialization;

namespace FlyingShadow.Core.DTO.Token;

public record TokenDetails([property: JsonPropertyName("token")] string Token, [property: JsonPropertyName("expiresAt")] DateTime ExpiresAt);