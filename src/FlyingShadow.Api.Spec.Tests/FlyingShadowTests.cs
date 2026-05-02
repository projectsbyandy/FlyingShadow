using FlyingShadow.Api.Spec.Tests.Support;
using FlyingShadow.Client;

namespace FlyingShadow.Api.Spec.Tests;

[Collection(OpenApiSpecCollection.Name)]
public class FlyingShadowTests
{
    private readonly FlyingShadowClient _authenticatedClient;

    public FlyingShadowTests(FlyingShadowClientBuilder factory, FakeUserLoginsProvider fakeUsers)
    {
        _authenticatedClient = factory.BuildAuthenticatedAsync(fakeUsers.ValidUser()).Result;
    }

    [Fact]
    public async Task Get_WithAuthenticatedUser_ReturnsShadows()
    {
        var shadows = await _authenticatedClient.Api.FlyingShadow.Shadows.GetAsync();
        
        // Assert
        Assert.NotNull(shadows);
        Assert.NotEmpty(shadows);
    }
}