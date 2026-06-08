using FlyingShadow.Api.Services;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;
using FlyingShadow.Core.Services.Mappers;
using Moq;

namespace FlyingShadow.Api.Tests.Services;

public class ShadowServiceTests : IClassFixture<ShadowDataFixture>
{
    private readonly IShadowService _sut;
    private readonly ShadowDataFixture _shadowDataFixture;
    private readonly Mock<IShadowRepository> _shadowRepositoryMock = new();
    private readonly Mock<IStealthMetricsRepository> _stealthMetricsRepositoryMock = new();
    private readonly IShadowDtoMapper _shadowDtoMapper = new ShadowDtoMapper();

    public ShadowServiceTests(ShadowDataFixture shadowDataFixture)
    {
        _shadowDataFixture = shadowDataFixture;
        _shadowRepositoryMock.Setup(s => s.GetAllAsync()).ReturnsAsync(Result<IEnumerable<Shadow>, Error>.Success(_shadowDataFixture.Shadows));
        _stealthMetricsRepositoryMock.Setup(s => s.GetAllAsync()).ReturnsAsync(Result<IEnumerable<StealthMetrics>, Error>.Success(_shadowDataFixture.StealthMetrics));
        _sut = new ShadowService(_shadowRepositoryMock.Object, _stealthMetricsRepositoryMock.Object,  _shadowDtoMapper);
    }
    
    [Fact]
    public async Task GetShadowDetails_ReturnsSuccess()
    {
        // Arrange / Act
        var shadowDetailsResult = await _sut.GetAllShadowDetailsAsync();
        
        // Assert
        Assert.True(shadowDetailsResult.IsSuccess);
    }
    
    [Fact]
    public async Task GetShadowDetails_ReturnsExpectedShadowDtoCount()
    {
        // Arrange / Act
        var shadowDetailsResult = await _sut.GetAllShadowDetailsAsync();
        
        // Assert
        Assert.True(shadowDetailsResult.IsSuccess);
        Assert.NotNull(shadowDetailsResult.Value);
        Assert.Equal(3, shadowDetailsResult.Value.Count);
    }
    
    [Fact]
    public async Task GetShadowDetails_CorrectlyMapsStealthMetrics()
    {
        // Arrange
        var expectedShadowDto = new ShadowDto()
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000036"),
            Clan = "Seven Swordsmen",
            CodeName = "Shadow Viper",
            Origin = "Mist Country",
            Rank = Rank.Danza,
            ShadowSkills = new ShadowDto.StealthMetricsDto()
            {
                ShadowBlendScore = 11,
                SilenceRating = 49,
                InvisibilityDurationMs = 3714,
                AcrobaticsLevel = AcrobaticsLevel.Advanced
            }
        };
        
        // Act
        var shadowDetailsResult = await _sut.GetAllShadowDetailsAsync();
        
        // Assert
        Assert.True(shadowDetailsResult.IsSuccess);
        Assert.NotNull(shadowDetailsResult.Value);
        Assert.Contains(shadowDetailsResult.Value, shadow => shadow.Equals(expectedShadowDto));
    }
    
    [Fact]
    public async Task GetShadowDetails_WithMissingStealthMetrics_OnlyReturnsShadowsWithSuccessfulStealthMetricsMapping()
    {
        // Arrange
        var localStealthMetrics = _shadowDataFixture.StealthMetrics.ToList();
        var metricToRemoveIndex = localStealthMetrics.FindIndex(m => m.InvisibilityDurationMs == 1022);
        localStealthMetrics.RemoveAt(metricToRemoveIndex);
        
        _stealthMetricsRepositoryMock.Setup(s => s.GetAllAsync()).ReturnsAsync(Result<IEnumerable<StealthMetrics>, Error>.Success(localStealthMetrics.AsEnumerable()));

        var expectedShadowIds = _shadowDataFixture.Shadows
            .Where(s => localStealthMetrics.Any(m => m.ShadowId == s.Id))
            .Select(s => s.Id)
            .ToList();
        
        
        // Act
        var shadowDetailsResult = await _sut.GetAllShadowDetailsAsync();
        
        // Assert
        Assert.NotNull(shadowDetailsResult.Value);
        Assert.Equal(2, shadowDetailsResult.Value.Count);
        Assert.DoesNotContain(shadowDetailsResult.Value, shadowDto => shadowDto.ShadowSkills.InvisibilityDurationMs.Equals(1022));
        Assert.All(expectedShadowIds, id =>
            Assert.Contains(shadowDetailsResult.Value, s => s.Id == id));
    }
    
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task GetShadowDetails_WithNoRepositoryData_ReturnsFailure(bool shadowListEmpty, bool stealthMetricsListEmpty)
    {
        // Arrange
        var localShadows = _shadowDataFixture.Shadows;
        var localStealthMetrics = _shadowDataFixture.StealthMetrics;
        
        if (shadowListEmpty)
            localShadows = Enumerable.Empty<Shadow>();
        
        if (stealthMetricsListEmpty) 
            localStealthMetrics = Enumerable.Empty<StealthMetrics>();
        
        _shadowRepositoryMock.Setup(s => s.GetAllAsync()).ReturnsAsync(Result<IEnumerable<Shadow>, Error>.Success(localShadows));
        _stealthMetricsRepositoryMock.Setup(s => s.GetAllAsync()).ReturnsAsync(Result<IEnumerable<StealthMetrics>, Error>.Success(localStealthMetrics));
        
        // Act
        var shadowDetailsResult = await _sut.GetAllShadowDetailsAsync();
        
        // Assert
        Assert.True(shadowDetailsResult.IsFailure);
        Assert.NotNull(shadowDetailsResult.Error);
        Assert.Equal(ErrorCode.UnableToProcessData, shadowDetailsResult.Error.Code);
        Assert.Equal("No Shadow Details mapped", shadowDetailsResult.Error.Message);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task GetShadowDetails_WhenRepositoryReturnsFailure_ReturnsFailure(bool isShadowResultSuccessful, bool isStealthMetricResultSuccessful)
    {
        // Arrange
        if (isShadowResultSuccessful is false)
            _shadowRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(Result<IEnumerable<Shadow>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, "Unable to fetch Shadows.")));

        if (isStealthMetricResultSuccessful is false)
            _stealthMetricsRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(
                Result<IEnumerable<StealthMetrics>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData,
                    "Unable to fetch stealth metrics.")));
        
        // Act
        var shadowDetailsResult = await _sut.GetAllShadowDetailsAsync();
        
        // Assert
        Assert.True(shadowDetailsResult.IsFailure);
        Assert.Equal(ErrorCode.UnableToRetrieveData, shadowDetailsResult.Error.Code);
        Assert.Equal("Unable to retrieve Shadow or Metric Data", shadowDetailsResult.Error.Message);
    }
    
    [Theory]
    [InlineData("Error 123: Unable to retrieve data")]
    [InlineData("Connection error")]
    public async Task GetShadowDetails_WithUnexpectedErrors_ReturnsUnexpectedError(string exceptionMessage)
    {
        // Arrange
        _shadowRepositoryMock.Setup(r => r.GetAllAsync()).Throws(new Exception(exceptionMessage));
        
        // Act / Assert
        var shadowDetailResult = await _sut.GetAllShadowDetailsAsync();
        
        // Assert
        Assert.True(shadowDetailResult.IsFailure);
        Assert.NotNull(shadowDetailResult.Error);
        Assert.Equal(ErrorCode.UnexpectedError, shadowDetailResult.Error.Code);
        Assert.Equal(exceptionMessage, shadowDetailResult.Error.Message);
    }

    [Fact]
    public async Task GetAllShadows_ReturnsMappedShadowToDto()
    {
        // arrange
        var shadowId = Guid.NewGuid();
        var sourceShadow = new Shadow()
        {
            Id = shadowId,
            Clan = "Flying Daggers",
            CodeName = "Blunt Stick",
            Origin = "Whisper Hollow",
            Rank = Rank.Oniwaban
        };

        var sourceStealthMetrics = new StealthMetrics()
        {
            Id = Guid.NewGuid(),
            ShadowId = shadowId,
            AcrobaticsLevel = AcrobaticsLevel.Advanced,
            InvisibilityDurationMs = 431,
            ShadowBlendScore = 12,
            SilenceRating = 52
        };
        
        _shadowRepositoryMock.Setup(s => s.GetAllAsync()).ReturnsAsync(Result<IEnumerable<Shadow>, Error>.Success(new List<Shadow> { sourceShadow }));
        _stealthMetricsRepositoryMock.Setup(s => s.GetAllAsync()).ReturnsAsync(Result<IEnumerable<StealthMetrics>, Error>.Success(new List<StealthMetrics> { sourceStealthMetrics }));
        
        // Act
        var shadowDetailsResults = await _sut.GetAllShadowDetailsAsync();
        
        // Assert
        Assert.NotNull(shadowDetailsResults.Value);
        Assert.Single(shadowDetailsResults.Value);

        var generatedShadowDto = shadowDetailsResults.Value.First();
        Assert.Equal(shadowId, generatedShadowDto.Id);
        Assert.Equal(sourceShadow.Clan, generatedShadowDto.Clan);
        Assert.Equal(sourceShadow.CodeName, generatedShadowDto.CodeName);
        Assert.Equal(sourceShadow.Origin, generatedShadowDto.Origin);
        Assert.Equal(sourceShadow.Rank, generatedShadowDto.Rank);
        Assert.Equal(sourceStealthMetrics.AcrobaticsLevel, generatedShadowDto.ShadowSkills.AcrobaticsLevel);
        Assert.Equal(sourceStealthMetrics.InvisibilityDurationMs, generatedShadowDto.ShadowSkills.InvisibilityDurationMs);
        Assert.Equal(sourceStealthMetrics.ShadowBlendScore, generatedShadowDto.ShadowSkills.ShadowBlendScore);
        Assert.Equal(sourceStealthMetrics.SilenceRating, generatedShadowDto.ShadowSkills.SilenceRating);
    }
}