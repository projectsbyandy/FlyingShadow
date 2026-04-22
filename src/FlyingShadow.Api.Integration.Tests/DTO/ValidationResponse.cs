using System.Text.Json.Serialization;

namespace FlyingShadow.Api.Integration.Tests.DTO;

internal record ValidationResponse
{
    [JsonPropertyName("errors")] 
    public Dictionary<string, IList<string>> Errors { get; set; } = new();
} 