using Ardalis.GuardClauses;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.Models.Ninja;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Repositories;

namespace FlyingShadow.Api.Repositories;

internal class FakeShadowRepository : WithMockData<IList<Shadow>>, IShadowRepository
{
    private readonly IList<Shadow> _shadows;

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
        return Result<IList<Shadow>, Error>.Success(_shadows);
    }
}