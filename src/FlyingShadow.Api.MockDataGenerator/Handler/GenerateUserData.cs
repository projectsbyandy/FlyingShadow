using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FlyingShadow.Api.MockDataGenerator.Utilities;
using FlyingShadow.Api.Models.ResultType;
using FlyingShadow.Api.Models.Users;

namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal static class GenerateUserData
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    
    public static async Task<Result<PipelineContext, int>> CredentialsAsync(PipelineContext context)
    {
        Console.WriteLine("MockDataGenerator: generating credentials...");
 
        var jwtSecret = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        
        var users = await FileReader.ReadAsync<IList<User>>("StaticData/users.json");
        var credentials = users.Select(u =>
        {
            var password = RandomPassword();
            var hashedPassword= BCrypt.Net.BCrypt.HashPassword(password, workFactor: 14);
 
            Console.WriteLine($"MockDataGenerator: Created {u.Email}");
 
            return new UserCredentials(u.UserId, u.Email, password, hashedPassword);
        }).ToList();
 
        return Result<PipelineContext, int>.Success(context with
        {
            JwtSecret   = jwtSecret,
            Credentials = credentials,
        });
    }

    public static async Task<Result<PipelineContext, int>> WriteFilesAsync(PipelineContext context)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(context.FakeDataDestinationPaths.LoginDetailsListPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(context.FakeDataDestinationPaths.UsersPath)!);
 
            var fakeLoginDetailsListRoot = new
            {
                jwt = new { secret = context.JwtSecret },
                fakeUsers = new
                {
                    loginDetailsList = context.Credentials.Select(c => new
                    {
                        email    = c.Email,
                        password = c.Password,
                    }),   
                }
            };

            var fakeUsersRoot = new
            {
                fakeUsers = new {
                    users = context.Credentials.Select(c => new
                    {
                        userId = c.UserId,
                        email = c.Email,
                        hashedPassword = c.HashedPassword
                    })
                }
            };
            
            await File.WriteAllTextAsync(
                context.FakeDataDestinationPaths.LoginDetailsListPath,
                JsonSerializer.Serialize(fakeLoginDetailsListRoot, JsonOpts));
 
            Console.WriteLine($"MockDataGenerator: written {context.FakeDataDestinationPaths.LoginDetailsListPath}");
            
            await File.WriteAllTextAsync(
                context.FakeDataDestinationPaths.UsersPath,
                JsonSerializer.Serialize(fakeUsersRoot, JsonOpts));
            
            Console.WriteLine($"MockDataGenerator: written {context.FakeDataDestinationPaths.UsersPath}");
 
            return Result<PipelineContext, int>.Success(context);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"MockDataGenerator: failed to write files — {ex.Message}");
            return Result<PipelineContext, int>.Failure(1);
        }
    }
    
    private static string RandomPassword(int length = 16)
    {
        const string chars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@#$";
        var maxValid = 256 - (256 % chars.Length);
        var sb = new StringBuilder(length);

        while (sb.Length < length)
        {
            foreach (var b in RandomNumberGenerator.GetBytes(length * 2))
            {
                if (b >= maxValid) continue;
                sb.Append(chars[b % chars.Length]);
                if (sb.Length == length) break;
            }
        }

        return sb.ToString();
    }
}