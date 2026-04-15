using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Services.Mappers;

namespace FlyingShadow.Core.Tests.Services.Mappers;

public class ShadowDtoMapperTests 
{
    private readonly IShadowDtoMapper _sut;

    private readonly IList<Shadow> Shadows = new List<Shadow>()
    {
        new()
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000034"),
            Clan = "Hidden Sound",
            CodeName = "Shadow Dragon",
            Origin = "Land of Sand",
            Rank = Rank.Toshiyama
        },
        new()
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000035"),
            Clan = "Shadow Hawk III",
            CodeName = "Shadow Hawk III",
            Origin = "Land of Wind",
            Rank = Rank.Oniwaban
        }
    };
    
    private readonly IList<StealthMetrics> StealthMetrics = new List<StealthMetrics>()
    {
        new()
        {
            Id = Guid.NewGuid(),
            ShadowId = Guid.Parse("550e8400-e29b-41d4-a716-000000000034"),
            ShadowBlendScore = 41,
            SilenceRating = 75,
            InvisibilityDurationMs = 2996,
            AcrobaticsLevel = AcrobaticsLevel.Intermediate
        },
        new()
        {
            Id = Guid.NewGuid(),
            ShadowId = Guid.Parse("550e8400-e29b-41d4-a716-000000000035"),
            ShadowBlendScore = 55,
            SilenceRating = 48,
            InvisibilityDurationMs = 1022,
            AcrobaticsLevel = AcrobaticsLevel.Beginner
        }
    };
    
    public ShadowDtoMapperTests()
    {
        _sut = new ShadowDtoMapper();
    }

    [Fact]
    public void ToList_With_Shadows_And_StealthMetrics_Returns_ShadowDTOs_List()
    {
        // Arrange
        var expectedDtos = new List<ShadowDto>()
        {
            new()
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000034"),
                Clan = "Hidden Sound",
                CodeName = "Shadow Dragon",
                Origin = "Land of Sand",
                Rank = Rank.Toshiyama,
                ShadowSkills = new ShadowDto.StealthMetricsDto
                {
                    ShadowBlendScore = 41,
                    SilenceRating = 75,
                    InvisibilityDurationMs = 2996,
                    AcrobaticsLevel = AcrobaticsLevel.Intermediate
                }
            },
            new()
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-000000000035"),
                Clan = "Shadow Hawk III",
                CodeName = "Shadow Hawk III",
                Origin = "Land of Wind",
                Rank = Rank.Oniwaban,
                ShadowSkills = new ShadowDto.StealthMetricsDto
                {
                    ShadowBlendScore = 55,
                    SilenceRating = 48,
                    InvisibilityDurationMs = 1022,
                    AcrobaticsLevel = AcrobaticsLevel.Beginner
                }
            }
        };
        
        // Act
        var shadowDtos = _sut.ToList(Shadows, StealthMetrics);
        
        // Assert
        Assert.Equal(expectedDtos, shadowDtos);
    }
    
    [Fact]
    public void ToSingle_With_Shadows_And_StealthMetrics_Returns_ShadowDTO()
    {
        // Arrange
        var firstShadow = Shadows.First();
        var firstStealthMetrics = StealthMetrics.First();
        //
        var mappedDto = _sut.ToSingle(firstShadow, firstStealthMetrics);
        
        // Assert
        Assert.Equal(firstShadow.Id, mappedDto.Id);
        Assert.Equal(firstShadow.Clan, mappedDto.Clan);
        Assert.Equal(firstShadow.CodeName, mappedDto.CodeName);
        Assert.Equal(firstShadow.Origin, mappedDto.Origin);
        Assert.Equal(firstShadow.Rank, mappedDto.Rank);
        Assert.Equal(firstStealthMetrics.AcrobaticsLevel, mappedDto.ShadowSkills.AcrobaticsLevel);
        Assert.Equal(firstStealthMetrics.ShadowBlendScore, mappedDto.ShadowSkills.ShadowBlendScore);
        Assert.Equal(firstStealthMetrics.InvisibilityDurationMs, mappedDto.ShadowSkills.InvisibilityDurationMs);
        Assert.Equal(firstStealthMetrics.SilenceRating, mappedDto.ShadowSkills.SilenceRating);
    }
    
    [Fact]
    public void ToList_Excludes_Shadows_With_No_Matching_StealthMetrics()
    {
        // Arrange
        var shadows = new List<Shadow> { Shadows[0] };
        var metrics = new List<StealthMetrics> { StealthMetrics[1] };

        // Act
        var result = _sut.ToList(shadows, metrics);

        // Assert
        Assert.Empty(result);
    }
}