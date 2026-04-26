using FlyingShadow.Api.MockDataGenerator.Models;
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
    
    public async Task<Result<PipelineContext, int>> CredentialsAsync(PipelineContext context)
    {
        Console.WriteLine("MockDataGenerator: generating credentials...");
        
        var users = await _fileManager.ReadAsync<IList<User>>("StaticData/users.json");
        var credentials = users.Select(u =>
        {
            var password = _secretGenerator.Password();
            var hashedPassword= _passwordHasher.Hash(password);
 
            Console.WriteLine($"MockDataGenerator: Created {u.Email}");
 
            return new UserCredentials(u.UserId, u.Email, password, hashedPassword);
        }).ToList();
 
        return Result<PipelineContext, int>.Success(context with
        {
            JwtKey   = _secretGenerator.Jwt(),
            Credentials = credentials,
        });
    }

    public async Task<Result<PipelineContext, int>> WriteJwtFileAsync(PipelineContext context)
    {
        return await ProcessAsync(context.MockDataOptions.FakeJwtPath, context, () => new
        {
            jwt = new { key = context.JwtKey }
        });
    }

    public async Task<Result<PipelineContext, int>> WriteLoginDetailsFileAsync(PipelineContext context)
    {
        return await ProcessAsync(context.MockDataOptions.FakeLoginDetailsListPath, context, () =>
            new
            {
                fakeUsers = new
                {
                    loginDetailsList = context.Credentials.Select(c => new
                    {
                        email = c.Email,
                        password = c.Password,
                    }),
                }
            }
        );
    }
    
    public async Task<Result<PipelineContext, int>> WriteUsersFileAsync(PipelineContext context)
    {
        return await ProcessAsync(context.MockDataOptions.FakeUsersPath, context, () => new
        {
            fakeUsers = new
            {
                users = context.Credentials.Select(c => new
                {
                    userId = c.UserId,
                    email = c.Email,
                    hashedPassword = c.HashedPassword
                })
            }
        });
    }

    private async Task<Result<PipelineContext, int>> ProcessAsync(string path, PipelineContext context, Func<object> generateObjectToWrite)
    {
        try
        {
            var asset = generateObjectToWrite();

            await CreateFileAssetsAsync(path, asset);
            
            return Result<PipelineContext, int>.Success(context);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"MockDataGenerator: failed to write {path} due to: {ex.Message}");
            return Result<PipelineContext, int>.Failure(1);
        }
    }

    private async Task CreateFileAssetsAsync(string filePath, dynamic objectToWrite)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new ArgumentNullException());
        await _fileManager.WriteAsync(filePath, objectToWrite);
        Console.WriteLine($"MockDataGenerator: written {filePath}");
    }
}