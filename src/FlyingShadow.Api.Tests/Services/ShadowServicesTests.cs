using FlyingShadow.Api.DTO.Shadow;
using FlyingShadow.Api.Models.Ninja;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Services;
using FlyingShadow.Api.Services.Internal;
using FlyingShadow.Api.Tests.Services.Fixtures;
using Moq;
using Xunit;

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
}