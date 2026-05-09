using Ardalis.GuardClauses;
using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;
using FlyingShadow.Api.MockDataGenerator.Utilities;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Utils;

namespace FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;

internal class UserDataGenerator : IUserDataGenerator
{
    private readonly IFileManager _fileManager;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISecretGenerator _secretGenerator;

    public UserDataGenerator(IFileManager fileManager, IPasswordHasher passwordHasher, ISecretGenerator secretGenerator)
    {
        _fileManager = fileManager;
        _passwordHasher = passwordHasher;
        _secretGenerator = secretGenerator;
    }
    
    public async Task<Result<PipelineContext, FailureCode>> CredentialsAsync(PipelineContext context)
    {
        Console.WriteLine("MockDataGenerator: generating credentials...");
        IReadOnlyList<UserCredentials> credentials;
        
        try
        {
            var users = await _fileManager.ReadAsync<IList<User>>("StaticData/users.json");
            Guard.Against.Zero(users.Count, nameof(users));
            
            credentials = users.Select(u =>
            {
                var password = _secretGenerator.Password();
                var hashedPassword= _passwordHasher.Hash(password);
 
                Console.WriteLine($"MockDataGenerator: Created {u.Email}");
 
                return new UserCredentials(u.UserId, u.Email, password, hashedPassword);
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("MockDataGenerator: failed to create credentials due to: " + ex.Message);
            return Result<PipelineContext, FailureCode>.Failure(FailureCode.Problem);
        }
 
        return Result<PipelineContext, FailureCode>.Success(context with
        {
            JwtKey   = _secretGenerator.Jwt(),
            Credentials = credentials,
        });
    }

    public async Task<Result<PipelineContext, FailureCode>> WriteJwtFileAsync(PipelineContext context)
    {
        return await ProcessAsync(context.MockDataOptions.FakeJwtPath, context, () => new
        {
            jwt = new { key = context.JwtKey }
        });
    }

    public async Task<Result<PipelineContext, FailureCode>> WriteLoginDetailsFileAsync(PipelineContext context)
    {
        return await ProcessAsync(context.MockDataOptions.FakeLoginDetailsListPath, context, () =>
            new
            {
                fakeUsers = new
                {
                    loginDetailsList = context.Credentials.Select(c => c.ToLoginDetails())
                }
            }
        );
    }
    
    public async Task<Result<PipelineContext, FailureCode>> WriteUsersFileAsync(PipelineContext context)
    {
        return await ProcessAsync(context.MockDataOptions.FakeUsersPath, context, () => new
        {
            fakeUsers = new
            {
                users = context.Credentials.Select(c => new
                {
                    c.UserId,
                    c.Email,
                    c.HashedPassword
                })
            }
        });
    }

    private async Task<Result<PipelineContext, FailureCode>> ProcessAsync(string path, PipelineContext context, Func<object> generateObjectToWrite)
    {
        try
        {
            var asset = generateObjectToWrite();

            await CreateFileAssetsAsync(path, asset);
            
            return Result<PipelineContext, FailureCode>.Success(context);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"MockDataGenerator: failed to write {path} due to: {ex.Message}");
            return Result<PipelineContext, FailureCode>.Failure(FailureCode.Problem);
        }
    }

    private async Task CreateFileAssetsAsync(string filePath, dynamic objectToWrite)
    {
        _fileManager.CreateDirectory(filePath);
        await _fileManager.WriteAsync(filePath, objectToWrite);
        Console.WriteLine($"MockDataGenerator: written {filePath}");
    }
}