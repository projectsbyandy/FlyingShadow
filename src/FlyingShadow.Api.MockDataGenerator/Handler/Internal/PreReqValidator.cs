using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Core.Models.ResultType;
using Microsoft.Extensions.Options;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Internal;

internal class PreReqValidator : IPreReqValidator
{
    private readonly MockDataOptions _options;

    public PreReqValidator(IOptions<MockDataOptions> options)
    {
        _options = options.Value;
    }

    // public Result<FakeDataDestinationPaths, int> CheckArguments()
    // {
    //     if (_options.FakeJwtPath)
    //     {
    //         Console.Error.WriteLine("MockDataGenerator: Error - Expected Usage: `MockDataGenerator <fakeJwtPath> <fakeUserRequestPath> <fakeUserPath> <fakeShadowPath> <fakeStealthMetricPath>`");
    //         return Result<FakeDataDestinationPaths, int>.Failure(1);
    //     }
    //
    //     var mappedPaths = new FakeDataDestinationPaths(args[0],  args[1], args[2], args[3], args[4]);
    //     
    //     return Result<FakeDataDestinationPaths, int>.Success(mappedPaths);
    // }
    
    public Task<Result<PipelineContext, int>> CheckFilesExistAsync()
    {
        var fakeLoginDetailsExists = File.Exists(_options.FakeLoginDetailsListPath);
        var fakeUsersExists  = File.Exists(_options.FakeJwtPath);
        var fakeShadowsExist = File.Exists(_options.FakeShadowsPath);
        var fakeStealthMetricsExists  = File.Exists(_options.FakeStealthMetricsPath);
        var fakeJwtExists = File.Exists(_options.FakeJwtPath);
 
        if (fakeLoginDetailsExists && fakeUsersExists && fakeShadowsExist && fakeStealthMetricsExists && fakeJwtExists)
        {
            Console.WriteLine("MockDataGenerator: files already exist, skipping.");
            return Task.FromResult(Result<PipelineContext, int>.Failure(0));
        }
 
        var context = new PipelineContext(
            MockDataOptions: _options,
            JwtKey: string.Empty,
            Credentials: []
        );
        
        return Task.FromResult(Result<PipelineContext, int>.Success(context));
    }
}