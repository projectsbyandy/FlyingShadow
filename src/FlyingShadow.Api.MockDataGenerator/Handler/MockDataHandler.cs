// ReSharper disable ConvertClosureToMethodGroup

using FlyingShadow.Api.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal static class MockDataHandler
{
    public static async Task<int> Process(string [] args)
    {
        var result = await ValidateArgs(args)
            .BindAsync(paths => CheckFilesExistAsync(paths))
            .BindAsync(context => GenerateData.CredentialsAsync(context))
            .BindAsync(context => GenerateData.WriteFilesAsync(context));
        
        return result.IsSuccess ? 0 : result.Value;
    }

    private static Result<(string FakeLoginDetailsPath, string FakeUsersPath), int> ValidateArgs(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("MockDataGenerator: Error - Expected Usage: `MockDataGenerator <fakeUserRequestPath> <fakeUserPath>`");
            return Result<(string, string), int>.Failure(1);
        }
 
        return Result<(string, string), int>.Success((args[0], args[1]));
    }

    private static Task<Result<PipelineContext, int>> CheckFilesExistAsync(
        (string FakeUserRequestsPath, string FakeUsersPath) paths)
    {
        var (fakeloginDetailsPath, fakeUsersPath) = paths;
 
        var fakeLoginDetailsExists = File.Exists(fakeloginDetailsPath);
        var fakeUsersExists  = File.Exists(fakeUsersPath);
 
        if (fakeLoginDetailsExists && fakeUsersExists)
        {
            Console.WriteLine("MockDataGenerator: files already exist, skipping.");
            return Task.FromResult(Result<PipelineContext, int>.Failure(0));
        }
 
        if (fakeLoginDetailsExists != fakeUsersExists)
        {
            Console.WriteLine("MockDataGenerator: WARNING – only one file exists, regenerating both for consistency.");
        }
 
        var context = new PipelineContext(
            FakeLoginDetailsListPath: fakeloginDetailsPath,
            FakeUsersPath: fakeUsersPath,
            JwtSecret: string.Empty,
            Credentials: []
        );
        
        return Task.FromResult(Result<PipelineContext, int>.Success(context));
    }
}