using FlyingShadow.Api.MockDataGenerator.Handler.Generate;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;
using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;

namespace FlyingShadow.Api.MockDataGenerator.Tests.Handler.Generate;

public class ShadowDataCopierTests : IDisposable
{
    private readonly IShadowDataCopier _sut;
    private readonly string _tempDir;
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
    private readonly string _sourceShadowPath;
    private readonly string _sourceStealthMetricsPath;
    private readonly PipelineContext _pipelineContext;
    
    public ShadowDataCopierTests()
    {
        _tempDir = Path.Combine(AppContext.BaseDirectory, "test-temp", $"{Guid.NewGuid().ToString()}");
        Directory.CreateDirectory(_tempDir);
        
        _sourceShadowPath = Path.Combine(_tempDir, "source-test-shadow.json");
        _sourceStealthMetricsPath = Path.Combine(_tempDir, "source-test-StealthMetrics.json");
            
        _pipelineContext = new PipelineContext(new MockDataOptions()
            {
                FakeJwtPath = "unused",
                FakeShadowsPath = Path.Combine(_tempDir, "destination-test-shadow.json"),
                FakeStealthMetricsPath = Path.Combine(_tempDir, "destination-test-StealthMetrics.json"),
                FakeLoginDetailsListPath = "unused",
                FakeUsersPath = "unused"
            }, 
            "unused", new List<UserCredentials>());
        
        _sut = new ShadowDataCopier();    
    }

    [Fact]
    public async Task Process_WithValidShadowDataFilePaths_CopiesSuccessfullyToDestination()
    {
        // Arrange

        _pipelineContext.StaticShadowData = _sourceShadowPath;
        _pipelineContext.StaticStealthMetricsData = _sourceStealthMetricsPath;
        
        await File.WriteAllTextAsync(_pipelineContext.StaticShadowData, "",  _cancellationToken);
        await File.WriteAllTextAsync(_pipelineContext.StaticStealthMetricsData, "",   _cancellationToken);
        
        // Act
        var result = _sut.Process(_pipelineContext);
        
        // Assert
        Assert.True(result.IsSuccess);
        
        Assert.True(File.Exists(_pipelineContext.MockDataOptions.FakeShadowsPath));
        Assert.True(File.Exists(_pipelineContext.MockDataOptions.FakeStealthMetricsPath));
    }

    [Fact]
    public void Process_WithInvalidSourcePath_ReturnsFailure()
    {
        // Arrange
        _pipelineContext.StaticShadowData = "DoesNotExist";
        _pipelineContext.StaticStealthMetricsData = "DoesNotExist";
        
        // Act
        var result = _sut.Process(_pipelineContext);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(FailureCode.Problem, result.Error);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
}