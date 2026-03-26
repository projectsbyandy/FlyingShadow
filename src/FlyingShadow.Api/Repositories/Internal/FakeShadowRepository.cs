using Ardalis.GuardClauses;
using FlyingShadow.Api.Models.Ninja;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Utils;

namespace FlyingShadow.Api.Repositories.Internal;

internal class FakeShadowRepository : WithMockData<IList<Shadow>>, IShadowRepository
{
    private static IList<Shadow>? _stealthMetrics;

    public FakeShadowRepository()
    {
        _stealthMetrics = LoadMockData();
    }

    public sealed override IList<Shadow> LoadMockData()
    {
        return Guard.Against.Null(ConfigReader.GetConfigurationSection<List<Shadow>>("FakeShadows"), 
            "Shadow Mock data has not been configured");
    }

    public Result<IList<Shadow>, Error> GetShadows()
    {
        throw new NotImplementedException();
    }
}