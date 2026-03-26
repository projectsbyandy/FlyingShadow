using FlyingShadow.Api.Integration.Tests.TestExtensions;
using FlyingShadow.Api.Models.Ninja;
using FlyingShadow.Api.Repositories;
using FlyingShadow.Api.Repositories.Internal;
using FlyingShadow.Api.Utils;

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