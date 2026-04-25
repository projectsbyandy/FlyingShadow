namespace FlyingShadow.Api.MockDataGenerator.Utilities;

internal interface IFileManager
{
    public Task<T> ReadAsync<T>(string filePath);
    public Task WriteAsync<T>(string filePath, T objectToWrite);
}