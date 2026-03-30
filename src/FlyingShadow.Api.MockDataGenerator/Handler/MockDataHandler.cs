// ReSharper disable ConvertClosureToMethodGroup

using FlyingShadow.Api.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal static class MockDataHandler
{
    public static async Task<int> Process(string [] args)
    {
        var result = await ValidateArgs(args)
            .BindAsync(paths => CheckFilesExistAsync(paths))
            .BindAsync(context => GenerateUserData.CredentialsAsync(context))
            .BindAsync(context => GenerateUserData.WriteFilesAsync(context))
            .Bind(context => CopyShadowData(context));
        
        return result.IsSuccess ? 0 : result.Value;
    }

    private static Result<FakeDataDestinationPaths, int> ValidateArgs(string[] args)
    {
        if (args.Length != 5)
        {
            Console.Error.WriteLine("MockDataGenerator: Error - Expected Usage: `MockDataGenerator <fakeJwtPath> <fakeUserRequestPath> <fakeUserPath> <fakeShadowPath> <fakeStealthMetricPath>`");
            return Result<FakeDataDestinationPaths, int>.Failure(1);
        }
 
        var mappedPaths = new FakeDataDestinationPaths(args[0],  args[1], args[2], args[3], args[4]);
        
        return Result<FakeDataDestinationPaths, int>.Success(mappedPaths);
    }

    private static Task<Result<PipelineContext, int>> CheckFilesExistAsync(
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

    private static Result<int, int> CopyShadowData(PipelineContext context)
    {
        try
        {
            File.Copy(Path.Combine(AppContext.BaseDirectory, "./StaticData/shadows.json"), context.FakeDataDestinationPaths.ShadowsPath, true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "./StaticData/stealthMetrics.json"), context.FakeDataDestinationPaths.StealthMetricsPath, true);
            Console.WriteLine($"MockDataGenerator: Copied Mock Shadow Data files");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MockDataGenerator: Problem copying Shadow Data {ex.Message}");
        
            return Result<int, int>.Failure(1);
        }
        
        return Result<int, int>.Success(0);
    }
}