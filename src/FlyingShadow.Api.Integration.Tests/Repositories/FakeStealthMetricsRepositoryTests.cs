using FlyingShadow.Api.Integration.Tests.TestExtensions;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Integration.Tests.Repositories;

public class FakeStealthMetricsRepositoryTests
{
    private static readonly IList<StealthMetrics> StealthMetrics = ConfigReader.GetConfigurationSection<List<StealthMetrics>>("fakeStealthMetrics");
    private readonly IStealthMetricsRepository _sut;
    
    public FakeStealthMetricsRepositoryTests()
    {
        _sut = new FakeStealthMetricsRepository();    
    }

    [MockDataFact]
    public void Verify_Mock_Data_Loaded()
    {
        Assert.True(StealthMetrics.Count > 0);
    }
}