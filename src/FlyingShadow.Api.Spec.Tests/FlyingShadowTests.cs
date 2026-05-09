using FlyingShadow.Api.Spec.Tests.Support;
using Ardalis.GuardClauses;
using FlyingShadow.Client.Models;

namespace FlyingShadow.Api.Spec.Tests;

[Collection(OpenApiSpecCollection.Name)]
public class FlyingShadowTests
{
    private readonly FlyingShadowClientBuilder _factory;
    private readonly FakeUserLoginsProvider _fakeUserLoginsProvider;
    
    public FlyingShadowTests(FlyingShadowClientBuilder factory, FakeUserLoginsProvider fakeUserLoginsProvider)
    {
        _factory = factory;
        _fakeUserLoginsProvider = fakeUserLoginsProvider;
    }

    [Fact]
    public async Task Get_WithAuthenticatedUser_ReturnsShadows()
    {
        // Arrange
        var authenticatedClient = await _factory.BuildAuthenticatedAsync(_fakeUserLoginsProvider.ValidUser());
        
        // Act
        var shadows = await authenticatedClient.Api.FlyingShadow.Shadows.GetAsync();
        
        // Assert
        Assert.NotNull(shadows);
        Assert.NotEmpty(shadows);
    }
    
    [Fact]
    public async Task Get_ReturnShadows_WithExpectedDetails()
    {
        // Arrange
        var authenticatedClient = await _factory.BuildAuthenticatedAsync(_fakeUserLoginsProvider.ValidUser());
        
        // Act
        var shadows = await authenticatedClient.Api.FlyingShadow.Shadows.GetAsync();
        
        // Assert
        Assert.NotNull(shadows);
        Assert.NotEmpty(shadows);
        
        var shadow = shadows.Find(s => Guard.Against.Null(s.CodeName).Equals("Shadow Wolf II"));
        Assert.NotNull(shadow);
        
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-000000000031"), shadow.Id);
        Assert.Equal("Hidden Rain", shadow.Clan);
        Assert.Equal("Land of Lightning", shadow.Origin);
        Assert.Equal(Rank.Toshiyama, shadow.Rank);
        Assert.NotNull(shadow.ShadowSkills);
        Assert.Equal(AcrobaticsLevel.Beginner, shadow.ShadowSkills.AcrobaticsLevel);
        Assert.Equal(3667, shadow.ShadowSkills.InvisibilityDurationMs);
        Assert.Equal(53, shadow.ShadowSkills.ShadowBlendScore);
        Assert.Equal(28, shadow.ShadowSkills.SilenceRating);
    }

    [Fact]
    public async Task Get_WithUnauthenticatedUser_Returns401()
    {
        // Arrange
        var client = _factory.BuildUnauthenticated();
        
        // Act
        var details = await Assert.ThrowsAsync<ProblemDetails>(() => client.Api.FlyingShadow.Shadows.GetAsync());
        
        // Assert
        Assert.Equal(401, details.ResponseStatusCode);
    }
}