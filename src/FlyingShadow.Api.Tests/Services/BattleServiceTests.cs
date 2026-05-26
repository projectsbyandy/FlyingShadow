using FlyingShadow.Api.Services;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services.Battle;
using FlyingShadow.Core.Services.Mappers;
using Moq;

namespace FlyingShadow.Api.Tests.Services;

public class BattleServiceTests : ShadowDataFixture
{
    private readonly IBattleService _sut;
    private readonly Mock<IShadowRepository> _shadowRepositoryMock = new();
    private readonly Mock<IStealthMetricsRepository> _stealthMetricsRepositoryMock = new();
    private readonly Mock<IShadowDtoMapper> _shadowDtoMapperMock = new();
    private readonly Mock<IBattleProcessor> _battleProcessorMock = new();
        
    public BattleServiceTests()
    {
        _shadowRepositoryMock.Setup(repo => repo.GetByCodeNameAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<Shadow, Error>.Success(Shadows.First()));

        _stealthMetricsRepositoryMock.Setup(repo => repo.GetByShadowIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<StealthMetrics, Error>.Success(StealthMetrics.First()));

        _shadowDtoMapperMock
            .Setup(mapper => mapper.ToSingle(It.IsAny<Shadow>(), It.IsAny<StealthMetrics>()))
            .Returns(GetShadowDTOs().First());
        
        _sut = new BattleService(_shadowRepositoryMock.Object, _shadowDtoMapperMock.Object, 
            _stealthMetricsRepositoryMock.Object, _battleProcessorMock.Object);
    }
    
    [Fact]
    public async Task Battle_WithValidShadowData_ReturnsBattleResponse()
    {
        // Arrange
        _battleProcessorMock
            .Setup(processor => processor.Process(It.IsAny<ShadowDto>(), It.IsAny<ShadowDto>()))
            .Returns(Result<BattleResponse, Error>.Success(BattleResponse));
        
        // Act
        var result = await _sut.BattleAsync("Test", "Test");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(BattleResponse, result.Value);
    }
    
    [Fact]
    public async Task Battle_WithInvalidShadowOneCodeName_ReturnsFailure()
    {
        // Arrange
        _shadowRepositoryMock
            .Setup(repo => repo.GetByCodeNameAsync(It.IsAny<string>()))
            .ReturnsAsync<string, IShadowRepository, Result<Shadow, Error>>(codeName => Result<Shadow, Error>.Failure(new Error(ErrorCode.NotFound, $"{codeName} not found")));

        const string shadowOneCodeName = "NonexistentCodeName";
        // Act
        var result = await _sut.BattleAsync(shadowOneCodeName, "shadowTwoExists");
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorCode.NotFound, result.Error.Code);
        Assert.Equal($"{shadowOneCodeName} not found", result.Error.Message);
    }
    
    [Fact]
    public async Task Battle_WithInvalidShadowTwoCodeName_ReturnsFailure()
    {
        // Arrange
        const string shadowTwoCodeName = "NonexistentCodeName";

        _shadowRepositoryMock
            .SetupSequence(repo => repo.GetByCodeNameAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<Shadow, Error>.Success(Shadows.First()))
            .ReturnsAsync(Result<Shadow, Error>.Failure(new Error(ErrorCode.NotFound, $"{shadowTwoCodeName} not found")));
        
        // Act
        var result = await _sut.BattleAsync("shadowOneExists", shadowTwoCodeName);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorCode.NotFound, result.Error.Code);
        Assert.Equal($"{shadowTwoCodeName} not found", result.Error.Message);
    }
    
    [Fact]
    public async Task Battle_WithInvalidStealthMetricsCodeName_ReturnsFailure()
    {
        // Arrange
        _stealthMetricsRepositoryMock
            .Setup(repo => repo.GetByShadowIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<StealthMetrics, Error>.Failure(new Error(ErrorCode.NotFound, "No Metrics here")));
        
        // Act
        var result = await _sut.BattleAsync("CodeNameExists", "Test");
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorCode.NotFound, result.Error.Code);
        Assert.Equal("No Metrics here", result.Error.Message);
    }

    [Fact]
    public async Task Battle_WithFailedBattleProcessor_ReturnsFailure()
    {
        // Arrange
        _battleProcessorMock
            .Setup(processor => processor.Process(It.IsAny<ShadowDto>(), It.IsAny<ShadowDto>()))
            .Returns(Result<BattleResponse, Error>.Failure(new Error(ErrorCode.UnableToProcessData, "Unable To Process Data")));
        
        // Act
        var result = await _sut.BattleAsync("Test", "Test");
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorCode.UnableToProcessData, result.Error.Code);
        Assert.Equal("Unable To Process Data", result.Error.Message);
    }
}