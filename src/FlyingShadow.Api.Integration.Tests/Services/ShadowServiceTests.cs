using FlyingShadow.Api.Integration.Tests.TestExtensions;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Services;
using FlyingShadow.Core.Services;

namespace FlyingShadow.Api.Integration.Tests.Services;

public class ShadowServiceTests
{
    private readonly IShadowService _sut = new ShadowService(new FakeShadowRepository(), new FakeStealthMetricsRepository());

    [MockDataFact]
    public void Verify_Successful_Shadow_Data_Retrieval()
    {
        // Arrange / Act
        var result = _sut.GetAllShadowDetails();
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value?.Count > 0);
    }
}