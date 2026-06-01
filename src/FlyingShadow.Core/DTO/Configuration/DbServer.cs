namespace FlyingShadow.Core.DTO.Configuration;

public record DbServer
{
    public bool IsMock { get; init; }
    public string? Host { get; set; }
    public int Port { get; init; }
    public string? Database { get; init; }
    public string? Username { get; init; }
    public string? Password  { get; init; }
    public int MinPoolSize { get; init; }
    public int MaxPoolSize { get; init; }
}