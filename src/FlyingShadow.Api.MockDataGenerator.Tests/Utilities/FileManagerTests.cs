using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Utilities;

namespace FlyingShadow.Api.MockDataGenerator.Tests.Utilities;

public class FileManagerTests : IDisposable
{
    private readonly IFileManager _sut;
    private readonly string _tempDir;
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
    private string PathFor(string name) => Path.Combine(_tempDir, name);

    public FileManagerTests()
    {
        _tempDir = Path.Combine(AppContext.BaseDirectory, "test-temp", $"{Guid.NewGuid().ToString()}");
        Directory.CreateDirectory(_tempDir);
        _sut = new FileManager();
    }

    [Fact]
    public async Task WriteAsync_ThenReadAsync_RoundTripsObject()
    {
        // Arrange
        var userCredentials = new UserCredentials(Guid.NewGuid(), "bob@test.com", "pass123", "8019hdajda82");
        var filePath = PathFor($"{Guid.NewGuid()}.json");
        
        // Act
        await _sut.WriteAsync(filePath, userCredentials);
        var readCredentials = await _sut.ReadAsync<UserCredentials>(filePath);
        
        // Assert
        Assert.Equal(userCredentials, readCredentials);
    }

    [Fact]
    public async Task WriteAsync_ProcessObjectWithIndented()
    {
        // Arrange
        var userCredentials = new UserCredentials(Guid.NewGuid(), "bob@test.com", "pass123", "8019hdajda82");
        var filePath = PathFor($"{Guid.NewGuid()}.json");

        // Act
        await _sut.WriteAsync(filePath, userCredentials);
        
        // Assert
        var details = await File.ReadAllTextAsync(filePath,  _cancellationToken);
        Assert.Contains("\n", details);
        Assert.Contains("  ", details);
    }
    
    [Fact]
    public async Task ReadAsync_WithMixedCasingObject_ReturnsObject()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var mixedCaseCredentials= new
        {
            uSerId = guid,
            EMAIL = "bob@test.com",
            passWORD = "test123",
            HaShEdPaSsWoRd = "ada891238103ada"
        };
        var filePath = PathFor($"{Guid.NewGuid()}.json");
        
        await _sut.WriteAsync(filePath, mixedCaseCredentials);
        
        // Act
        var credentials = await _sut.ReadAsync<UserCredentials>(filePath);
        
        // Assert
        Assert.Equal(new UserCredentials(guid, "bob@test.com", "test123", "ada891238103ada"), credentials);
    }
    
    [Fact]
    public async Task ReadAsync_WithInCorrectObjectProcessing_ThrowsArgumentNullException()
    {
        // Arrange
        var filePath = PathFor($"{Guid.NewGuid()}.json");
        
        await File.WriteAllTextAsync(filePath, "null", _cancellationToken);
        
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ReadAsync<string>(filePath));
        
        // Assert
        Assert.Equal("results", exception.ParamName);
        Assert.Contains("Value cannot be null.", exception.Message);
    }

    [Fact]
    public async Task ReadAsync_WithNonExistentFile_ThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentfilePath = PathFor($"DoesNotExist_{Guid.NewGuid()}");
        
        // Act
        var exception = await Assert.ThrowsAsync<FileNotFoundException>(() => _sut.ReadAsync<UserCredentials>(nonExistentfilePath));
        
        // Assert
        Assert.Contains("File not found:", exception.Message);
        Assert.Contains(nonExistentfilePath, exception.Message);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
}