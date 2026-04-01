using FlyingShadow.Api.Integration.Tests.TestExtensions;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Integration.Tests.Repositories;

public class FakeShadowRepositoryTests
{
    private static readonly IList<Shadow> FakeShadows = ConfigReader.GetConfigurationSection<List<Shadow>>("fakeShadows");
    private readonly IShadowRepository _sut;
    
    public FakeShadowRepositoryTests()
    {
        _sut = new FakeShadowRepository();    
    }

    [MockDataFact]
    public void Verify_Mock_Data_Loaded()
    {
        Assert.True(FakeShadows.Count > 0);
    }
}