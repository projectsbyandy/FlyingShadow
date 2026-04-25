using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Internal;

internal class PreReqValidator : IPreReqValidator
{
    public Result<FakeDataDestinationPaths, int> CheckArguments(string[] args)
    {
        if (args.Length != 5)
        {
            Console.Error.WriteLine("MockDataGenerator: Error - Expected Usage: `MockDataGenerator <fakeJwtPath> <fakeUserRequestPath> <fakeUserPath> <fakeShadowPath> <fakeStealthMetricPath>`");
            return Result<FakeDataDestinationPaths, int>.Failure(1);
        }
 
        var mappedPaths = new FakeDataDestinationPaths(args[0],  args[1], args[2], args[3], args[4]);
        
        return Result<FakeDataDestinationPaths, int>.Success(mappedPaths);
    }
    
    public Task<Result<PipelineContext, int>> CheckFilesExistAsync(
        FakeDataDestinationPaths destinationPaths)
    {
        var fakeLoginDetailsExists = File.Exists(destinationPaths.LoginDetailsListPath);
        var fakeUsersExists  = File.Exists(destinationPaths.UsersPath);
        var fakeShadowsExist = File.Exists(destinationPaths.ShadowsPath);
        var fakeStealthMetricsExists  = File.Exists(destinationPaths.StealthMetricsPath);
        var fakeJwtExists = File.Exists(destinationPaths.JwtKeyPath);
 
        if (fakeLoginDetailsExists && fakeUsersExists && fakeShadowsExist && fakeStealthMetricsExists && fakeJwtExists)
        {
            Console.WriteLine("MockDataGenerator: files already exist, skipping.");
            return Task.FromResult(Result<PipelineContext, int>.Failure(0));
        }
 
        var context = new PipelineContext(
            FakeDataDestinationPaths: destinationPaths,
            JwtKey: string.Empty,
            Credentials: []
        );
        
        return Task.FromResult(Result<PipelineContext, int>.Success(context));
    }
}