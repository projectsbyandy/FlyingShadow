using System.Text.Json;

namespace FlyingShadow.Api.MockDataGenerator.Utilities;

internal class FileManager : IFileManager
{
    public async Task<T> ReadAsync<T>(string filePath)
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, filePath);
        
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"JSON file not found: {filePath}");

        await using var stream = File.OpenRead(fullPath);

        var results = await JsonSerializer.DeserializeAsync<T>(stream, ReadJsonOptions);

        ArgumentNullException.ThrowIfNull(results);
        
        return results;
    }

    public async Task WriteAsync<T>(string filePath, T objectToWrite)
    {
        await File.WriteAllTextAsync(
            filePath,
            JsonSerializer.Serialize(objectToWrite, WriteJsonOpts));
    }
   
    private static readonly JsonSerializerOptions WriteJsonOpts = new() { WriteIndented = true };

    private static readonly JsonSerializerOptions ReadJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}