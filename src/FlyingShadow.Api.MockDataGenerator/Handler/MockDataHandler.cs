// ReSharper disable ConvertClosureToMethodGroup

using FlyingShadow.Api.MockDataGenerator.Handler.Generate;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal class MockDataHandler
{
    private readonly IPreReqValidator _preReqValidator;
    private readonly IUserDataGenerator _userDataGenerator;
    private readonly IShadowDataCopier _shadowDataCopier;

    public MockDataHandler(IPreReqValidator preReqValidator, IUserDataGenerator userDataGenerator, IShadowDataCopier shadowDataCopier)
    {
        _userDataGenerator = userDataGenerator;
        _preReqValidator = preReqValidator;
        _shadowDataCopier = shadowDataCopier;
    }

    public async Task<int> Process()
    {
        var result = await _preReqValidator.CheckFilesExistAsync()
            .BindAsync(_userDataGenerator.CredentialsAsync)
            .BindAsync(_userDataGenerator.WriteJwtFileAsync)
            .BindAsync(_userDataGenerator.WriteLoginDetailsFileAsync)
            .BindAsync(_userDataGenerator.WriteUsersFileAsync)
            .Bind(_shadowDataCopier.Process);
        
        return result.IsSuccess ? (int)result.Value : (int)result.Error;
    }
}