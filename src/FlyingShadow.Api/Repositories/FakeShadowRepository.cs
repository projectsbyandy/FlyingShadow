using Ardalis.GuardClauses;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class FakeShadowRepository : WithMockData<IList<Shadow>>, IShadowRepository
{
    private static IList<Shadow>? _shadows;

    public FakeShadowRepository()
    {
        _shadows = LoadMockData();
    }

    public sealed override IList<Shadow> LoadMockData()
    {
        return Guard.Against.Null(ConfigReader.GetConfigurationSection<List<Shadow>>("FakeShadows"), 
            "Shadow Mock data has not been configured");
    }

    public Result<IList<Shadow>, Error> GetAll()
    {
        return _shadows is null
            ? Result<IList<Shadow>, Error>.Failure(new Error("UNABLE_TO_LOAD_SHADOWS", "Unable to fetch Shadows."))
            : Result<IList<Shadow>, Error>.Success(_shadows);
    }
}