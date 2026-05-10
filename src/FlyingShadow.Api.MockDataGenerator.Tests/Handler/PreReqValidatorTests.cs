using FlyingShadow.Api.MockDataGenerator.Handler;
using FlyingShadow.Api.MockDataGenerator.Handler.Internal;
using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;
using Microsoft.Extensions.Options;
using Moq;

namespace FlyingShadow.Api.MockDataGenerator.Tests.Handler;

public class PreReqValidatorTests : IDisposable
{
    private IPreReqValidator _sut;
    private readonly Mock<IOptions<MockDataOptions>> _mockOptions = new();
    private readonly string _tempDir;
    
    public PreReqValidatorTests()
    {
        _tempDir = Path.Combine(AppContext.BaseDirectory, AppContext.BaseDirectory, "test-temp", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public async Task CheckFilesExistAsync_WithNoExistingFiles_ReturnsValidPipelineContext()
    {
        // Arrange
        _mockOptions.Setup(o => o.Value).Returns(MockDataOptions);
        _sut = new PreReqValidator(_mockOptions.Object);

        // Act
        var result = await _sut.CheckFilesExistAsync();
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(MockDataOptions, result.Value.MockDataOptions);
        Assert.Equal(string.Empty, result.Value.JwtKey);
        Assert.Empty(result.Value.Credentials);
    }
    
    [Fact]
    public async Task CheckFilesExistAsync_WithExistingFile_ReturnsWarning()
    {
        // Arrange
        var tempFile = Path.Combine(_tempDir, "PreReqValidatorTest.json");
        File.Create(tempFile).Close();
        
        var options = MockDataOptions with
        {
            FakeJwtPath = tempFile
        };
        
        _mockOptions.Setup(o => o.Value).Returns(options);
        _sut = new PreReqValidator(_mockOptions.Object);
        
        // Act
        var result = await _sut.CheckFilesExistAsync();
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(FailureCode.Warning, result.Error);
    }

    private MockDataOptions MockDataOptions => new()
    {
        FakeJwtPath = "DoesNotExist",
        FakeLoginDetailsListPath = "DoesNotExist",
        FakeShadowsPath = "DoesNotExist",
        FakeUsersPath = "DoesNotExist",
        FakeStealthMetricsPath = "DoesNotExist",
    };

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
}