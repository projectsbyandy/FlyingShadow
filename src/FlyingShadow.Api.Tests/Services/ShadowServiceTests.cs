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

public class ShadowServiceTests : ShadowDataFixture
{
    private IShadowService _sut;
    private readonly Mock<IShadowRepository> _shadowRepositoryMock = new();
    private readonly Mock<IStealthMetricsRepository> _stealthMetricsRepositoryMock = new();
    private readonly IShadowDtoMapper _shadowDtoMapper = new ShadowDtoMapper();

    public ShadowServiceTests()
    {
        _shadowRepositoryMock.Setup(s => s.GetAll()).Returns(Result<IList<Shadow>, Error>.Success(Shadows));
        _stealthMetricsRepositoryMock.Setup(s => s.GetAll()).Returns(Result<IList<StealthMetrics>, Error>.Success(StealthMetrics));
        _sut = new ShadowService(_shadowRepositoryMock.Object, _stealthMetricsRepositoryMock.Object,  _shadowDtoMapper);
    }
    
    [Fact]
    public void Verify_GetShadowDetails_Returns_Success()
    {
        // Arrange / Act
        var shadowDetailsResult = _sut.GetAllShadowDetails();
        
        // Assert
        Assert.True(shadowDetailsResult.IsSuccess);
    }
    
    [Fact]
    public void Verify_GetShadowDetails_Returns_Expected_ShadowDto_Count()
    {
        // Arrange / Act
        var shadowDetailsResult = _sut.GetAllShadowDetails();
        
        // Assert
        Assert.NotNull(shadowDetailsResult.Value);
        Assert.Equal(3, shadowDetailsResult.Value.Count);
    }
    
    [Fact]
    public void Verify_GetShadowDetails_Correctly_Maps_StealthMetrics()
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
        var shadowDetailsResult = _sut.GetAllShadowDetails();
        
        // Assert
        Assert.NotNull(shadowDetailsResult.Value);
        Assert.Contains(shadowDetailsResult.Value, shadow => shadow.Equals(expectedShadowDto));
    }
    
    [Fact]
    public void Verify_GetShadowDetails_Only_Returns_Shadows_With_Successful_StealthMetrics_Mapping()
    {
        // Arrange
        var metricToRemoveIndex = StealthMetrics.ToList().FindIndex(m => m.InvisibilityDurationMs == 1022);
        StealthMetrics.RemoveAt(metricToRemoveIndex);
        
        var expectedShadowIds = Shadows
            .Where(s => StealthMetrics.Any(m => m.ShadowId == s.Id))
            .Select(s => s.Id)
            .ToList();
        
        // Act
        var shadowDetailsResult = _sut.GetAllShadowDetails();
        
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
    public void Verify_GetShadowDetails_Returns_Error_When_No_Repository_Data_Empty(bool shadowListEmpty, bool stealthMetricsListEmpty)
    {
        // Arrange
        if (shadowListEmpty)
            Shadows.Clear();
        
        if(stealthMetricsListEmpty) 
            StealthMetrics.Clear();
        
        // Act
        var shadowDetailsResult = _sut.GetAllShadowDetails();
        
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
    public void Verify_GetShadowDetails_Returns_Error_When_Repository_Result_Returns_Error(bool isShadowResultSuccessful, bool isStealthMetricResultSuccessful)
    {
        // Arrange
        if (isShadowResultSuccessful is false)
            _shadowRepositoryMock.Setup(r => r.GetAll ()).Returns(Result<IList<Shadow>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData, "Unable to fetch Shadows.")));

        if (isStealthMetricResultSuccessful is false)
            _stealthMetricsRepositoryMock.Setup(r => r.GetAll()).Returns(
                Result<IList<StealthMetrics>, Error>.Failure(new Error(ErrorCode.UnableToRetrieveData,
                    "Unable to fetch stealth metrics.")));
        
        // Act
        var shadowDetailsResult = _sut.GetAllShadowDetails();
        
        // Assert
        Assert.True(shadowDetailsResult.IsFailure);
        Assert.Equal(ErrorCode.UnableToRetrieveData, shadowDetailsResult.Error.Code);
        Assert.Equal("Unable to retrieve Shadow or Metric Data", shadowDetailsResult.Error.Message);
    }
    
    [Theory]
    [InlineData("Error 123: Unable to retrieve data")]
    [InlineData("Connection error")]
    public void Verify_GetShadowDetails_Handles_Unexpected_Error(string exceptionMessage)
    {
        // Arrange
        _shadowRepositoryMock.Setup(r => r.GetAll ()).Throws(new Exception(exceptionMessage));
        
        // Act / Assert
        var shadowDetailResult = _sut.GetAllShadowDetails();
        
        // Assert
        Assert.True(shadowDetailResult.IsFailure);
        Assert.NotNull(shadowDetailResult.Error);
        Assert.Equal(ErrorCode.UnexpectedError, shadowDetailResult.Error.Code);
        Assert.Equal(exceptionMessage, shadowDetailResult.Error.Message);
    }

    [Fact]
    public void Verify_Map_Shadow_To_Dto()
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
        
        _shadowRepositoryMock.Setup(s => s.GetAll()).Returns(Result<IList<Shadow>, Error>.Success(new List<Shadow> { sourceShadow }));
        _stealthMetricsRepositoryMock.Setup(s => s.GetAll()).Returns(Result<IList<StealthMetrics>, Error>.Success(new List<StealthMetrics> { sourceStealthMetrics }));
        
        // Act
        var shadowDetailsResults = _sut.GetAllShadowDetails();
        
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