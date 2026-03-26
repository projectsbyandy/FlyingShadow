using FlyingShadow.Api.Models.Ninja;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Repositories.Internal;
using FlyingShadow.Api.Utils;

namespace FlyingShadow.Api.Integration.Tests.Repositories;

public class FakeStealthMetricsRepositoryTests
{
    private static readonly IList<StealthMetrics> StealthMetrics = ConfigReader.GetConfigurationSection<List<StealthMetrics>>("fakeStealthMetrics");
    private readonly IStealthMetricsRepository _sut;
    
    public FakeStealthMetricsRepositoryTests()
    {
        _sut = new FakeStealthMetricsRepository();    
    }

    [Fact]
    public void Verify_Mock_Data_Loaded()
    {
        Assert.True(StealthMetrics.Count > 0);
    }
}