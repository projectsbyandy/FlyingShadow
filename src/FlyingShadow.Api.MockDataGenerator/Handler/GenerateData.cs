using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FlyingShadow.Api.Models.ResultType;

namespace FlyingShadow.Api.MockDataGenerator.Handler;

internal static class GenerateData
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    
    private static readonly (string UserId, string Email)[] StaticUsers =
    [
        ("820bad3a-58cd-4f6f-970c-11e7aca30b89", "demo_user@sample.org"),
        ("58d6bc7b-06a7-421e-8f38-4f0e9d09420d", "john.doe@sample.org"),
    ];

    public static Task<Result<PipelineContext, int>> CredentialsAsync(PipelineContext context)
    {
        Console.WriteLine("MockDataGenerator: generating credentials...");
 
        var jwtSecret = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
 
        var credentials = StaticUsers.Select(u =>
        {
            var password = RandomPassword();
            var hashedPassword= BCrypt.Net.BCrypt.HashPassword(password, workFactor: 14);
 
            Console.WriteLine($"MockDataGenerator: Created {u.Email}");
 
            return new UserCredentials(u.UserId, u.Email, password, hashedPassword);
        }).ToList();
 
        return Task.FromResult(Result<PipelineContext, int>.Success(context with
        {
            JwtSecret   = jwtSecret,
            Credentials = credentials,
        }));
    }

    public static async Task<Result<int, int>> WriteFilesAsync(PipelineContext context)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(context.FakeLoginDetailsListPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(context.FakeUsersPath)!);
 
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
                context.FakeLoginDetailsListPath,
                JsonSerializer.Serialize(fakeLoginDetailsListRoot, JsonOpts));
 
            await File.WriteAllTextAsync(
                context.FakeUsersPath,
                JsonSerializer.Serialize(fakeUsersRoot, JsonOpts));
 
            Console.WriteLine($"MockDataGenerator: written {context.FakeLoginDetailsListPath}");
            Console.WriteLine($"MockDataGenerator: written {context.FakeUsersPath}");
 
            return Result<int, int>.Success(0);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"MockDataGenerator: failed to write files — {ex.Message}");
            return Result<int, int>.Failure(1);
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