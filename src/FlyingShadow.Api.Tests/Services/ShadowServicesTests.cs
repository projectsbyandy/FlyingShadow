using FlyingShadow.Api.Services;
using FlyingShadow.Api.Tests.Services.Fixtures;
using FlyingShadow.Core.DTO.Shadow;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;
using FlyingShadow.Core.Services;
using Moq;

namespace FlyingShadow.Api.Tests.Services;

public class ShadowServicesTests
{
    private IShadowService _sut;
    private ShadowDataFixture _shadowDataFixture;
    private readonly Mock<IShadowRepository> _shadowRepositoryMock = new();
    private readonly Mock<IStealthMetricsRepository> _stealthMetricsRepositoryMock = new();

    public ShadowServicesTests()
    {
        
        _shadowDataFixture = new ShadowDataFixture();
        _shadowRepositoryMock.Setup(s => s.GetAll()).Returns(Result<IList<Shadow>, Error>.Success(_shadowDataFixture.Shadows));
        _stealthMetricsRepositoryMock.Setup(s => s.GetAll()).Returns(Result<IList<StealthMetrics>, Error>.Success(_shadowDataFixture.StealthMetrics));
        
        _sut = new ShadowService(_shadowRepositoryMock.Object, _stealthMetricsRepositoryMock.Object);
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
        var metricToRemoveIndex = _shadowDataFixture.StealthMetrics.ToList().FindIndex(m => m.InvisibilityDurationMs == 1022);
        _shadowDataFixture.StealthMetrics.RemoveAt(metricToRemoveIndex);
        
        var expectedShadowIds = _shadowDataFixture.Shadows
            .Where(s => _shadowDataFixture.StealthMetrics.Any(m => m.ShadowId == s.Id))
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
            _shadowDataFixture.Shadows.Clear();
        
        if(stealthMetricsListEmpty) 
            _shadowDataFixture.StealthMetrics.Clear();
        
        // Act
        var shadowDetailsResult = _sut.GetAllShadowDetails();
        
        // Assert
        Assert.False(shadowDetailsResult.IsSuccess);
        Assert.Null(shadowDetailsResult.Value);
        Assert.NotNull(shadowDetailsResult.Error);
        Assert.Equal("NO_SHADOW_DETAILS_MAPPED", shadowDetailsResult.Error.Code);
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
            _shadowRepositoryMock.Setup(r => r.GetAll ()).Returns(Result<IList<Shadow>, Error>.Failure(new Error("UNABLE_TO_LOAD_SHADOWS", "Unable to fetch Shadows.")));

        if (isStealthMetricResultSuccessful is false)
            _stealthMetricsRepositoryMock.Setup(r => r.GetAll()).Returns(
                Result<IList<StealthMetrics>, Error>.Failure(new Error("UNABLE_TO_LOAD_STEALTH_METRICS",
                    "Unable to fetch stealth metrics.")));
        
        // Act
        var shadowDetailsResult = _sut.GetAllShadowDetails();
        
        // Assert
        Assert.False(shadowDetailsResult.IsSuccess);
        Assert.Null(shadowDetailsResult.Value);
        Assert.NotNull(shadowDetailsResult.Error);
        Assert.Equal("NO_SHADOW_OR_METRIC_DATA", shadowDetailsResult.Error.Code);
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
        Assert.False(shadowDetailResult.IsSuccess);
        Assert.NotNull(shadowDetailResult.Error);
        Assert.Equal("UNEXPECTED_ERROR", shadowDetailResult.Error.Code);
        Assert.Equal(exceptionMessage, shadowDetailResult.Error.Message);
    }
}