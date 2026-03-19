// ReSharper disable ConvertClosureToMethodGroup

using FlyingShadow.Api.DTO.ResultType;

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

    private static Result<(string FakeUsersPath, string FakeDbUsersPath), int> ValidateArgs(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("MockDataGenerator: Error - Expected Usage: `MockDataGenerator <fakeUsersPath> <userStorePath>`");
            return Result<(string, string), int>.Failure(1);
        }
 
        return Result<(string, string), int>.Success((args[0], args[1]));
    }

    private static Task<Result<PipelineContext, int>> CheckFilesExistAsync(
        (string FakeUsersPath, string FakeDbUsersPath) paths)
    {
        var (fakeUsersPath, fakeDbUsersPath) = paths;
 
        var fakeUsersExists = File.Exists(fakeUsersPath);
        var fakeDbUsersExists  = File.Exists(fakeDbUsersPath);
 
        if (fakeUsersExists && fakeDbUsersExists)
        {
            Console.WriteLine("MockDataGenerator: files already exist, skipping.");
            return Task.FromResult(Result<PipelineContext, int>.Failure(0));
        }
 
        if (fakeUsersExists != fakeDbUsersExists)
        {
            Console.WriteLine("MockDataGenerator: WARNING – only one file exists, regenerating both for consistency.");
        }
 
        var context = new PipelineContext(
            FakeUsersPath: fakeUsersPath,
            FakeDbUsersPath: fakeDbUsersPath,
            JwtSecret: string.Empty,
            Credentials: []
        );
        
        return Task.FromResult(Result<PipelineContext, int>.Success(context));
    }
}