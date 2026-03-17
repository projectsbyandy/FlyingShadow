using System.Text.Json;

namespace FlyingShadow.Api.Utils;

internal static class FileReader
{
    public static async Task<T> ReadAsync<T>(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"JSON file not found: {filePath}");

        await using var stream = File.OpenRead(filePath);

        var results = await JsonSerializer.DeserializeAsync<T>(stream, Options);

        ArgumentNullException.ThrowIfNull(results);
        
        return results;
    }
    
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };
}