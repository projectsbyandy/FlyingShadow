using System.Text.Json;

namespace FlyingShadow.Api.MockDataGenerator.Utilities;

internal static class FileReader
{
    public static async Task<T> ReadAsync<T>(string filePath)
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, filePath);
        
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"JSON file not found: {filePath}");

        await using var stream = File.OpenRead(fullPath);

        var results = await JsonSerializer.DeserializeAsync<T>(stream, Options);

        ArgumentNullException.ThrowIfNull(results);
        
        return results;
    }
    
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };
}