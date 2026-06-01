// using System.Diagnostics;
// using Ardalis.GuardClauses;
// using Dapper;
// using FlyingShadow.Api.Utils;
// using FlyingShadow.Core.DTO.Configuration;
// using FlyingShadow.Core.Models.Ninja;
// using FlyingShadow.Core.Models.Users;
// using Npgsql;
// using Testcontainers.PostgreSql;
//
// namespace FlyingShadow.Api.Integration.Tests;
//
// public class PostgresTest
// {
//     private readonly ITestOutputHelper _output;
//
//     public PostgresTest(ITestOutputHelper output)
//     {
//         _output = output;
//     }
//
//     [Fact]
//     public async Task PostgreSqlTestAsync()
//     {
//
//         var stopWatch = new Stopwatch();
//         _output.WriteLine($"Starting {nameof(PostgresTest)}: {stopWatch.ElapsedMilliseconds} ms");
//         stopWatch.Start();
//         var cancellationToken = new CancellationTokenSource(TimeSpan.FromHours(1)).Token;
//         PostgreSqlContainer postgres = new PostgreSqlBuilder("postgres:17")
//             .WithPortBinding(47777, 5432)
//             .WithDatabase("flyingshadow2")
//             .WithUsername("tester")
//             .WithPassword("pass123")
//             .Build();
//
//        var shadows = Guard.Against.Null(ConfigReader.GetConfigurationSection<List<Shadow>>("FakeShadows"), 
//             "Shadow Mock data has not been configured");
//        
//        var stealthMetrics = Guard.Against.Null(ConfigReader.GetConfigurationSection<List<StealthMetrics>>("FakeStealthMetrics"), 
//            "Stealth Metrics Mock data has not been configured");
//      
//        var users = Guard.Against.Null(ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers").Users, 
//            "Users Mock data has not been configured");
//        
//        var jwt = Guard.Against.Null(ConfigReader.GetConfigurationSection<Jwt>("Jwt").Key, 
//            "JWT Mock key has not been configured");
//        
//         await postgres.StartAsync(cancellationToken);
//
//         var connectionString = postgres.GetConnectionString();
//         await CreateSchemaAsync(connectionString);
//         _output.WriteLine($"Connection string: {connectionString}");
//         
//         await SeedShadowsAsync(shadows, connectionString);
//         await SeedStealthMetricsAsync(stealthMetrics, connectionString);
//         await SeedUsersAsync(users, connectionString);
//         await SeedTestSupportAsync(jwt, connectionString);
//             
//         stopWatch.Stop();
//         _output.WriteLine($"Complete {nameof(PostgresTest)}: {stopWatch.ElapsedMilliseconds} ms");
//         await postgres.StopAsync(cancellationToken);
//
//     }
//     
//     private async Task CreateSchemaAsync(string connectionString)
//     {
//         const string shadowsTableQuery = 
//             """
//                CREATE TABLE IF NOT EXISTS shadows (
//                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
//                    code_name TEXT NOT NULL,
//                    clan TEXT NOT NULL,
//                    origin TEXT NOT NULL,
//                    rank TEXT NOT NULL
//                );
//             """;
//         
//         const string metricsTableQuery = 
//             """
//                CREATE TABLE IF NOT EXISTS stealthmetrics (
//                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
//                    shadow_id UUID NOT NULL,
//                    shadow_blend_score INT NOT NULL,
//                    silence_rating INT NOT NULL,
//                    invisibility_duration_ms INT NOT NULL,
//                    acrobatics_level TEXT NOT NULL
//                );
//             """;
//
//         const string usersTableQuery =
//             """
//                CREATE TABLE IF NOT EXISTS users (
//                    user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
//                    email TEXT NOT NULL,
//                    hashed_password TEXT NOT NULL
//                );
//             """;
//         
//         const string testSupportTableQuery =
//             """
//                CREATE TABLE IF NOT EXISTS testsupport (
//                    jwt TEXT NOT NULL
//                );
//             """;
//         var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
//
//         await using var conn = await dataSource.OpenConnectionAsync();
//         await conn.ExecuteAsync(shadowsTableQuery);
//         await conn.ExecuteAsync(metricsTableQuery);
//         await conn.ExecuteAsync(usersTableQuery);
//         await conn.ExecuteAsync(testSupportTableQuery);
//     }
//     
//     private async Task SeedShadowsAsync(IList<Shadow> shadows, string connectionString)
//     {
//         const string shadowsQuery = 
//             """
//                INSERT INTO shadows (id, code_name, clan, origin, rank)
//                SELECT * FROM UNNEST(
//                     @id::uuid[],
//                     @code_name::text[],
//                     @clan::text[],
//                     @origin::text[],
//                     @rank::text[]
//                )
//             """;
//         
//         var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
//
//         await using var conn = await dataSource.OpenConnectionAsync();
//         await conn.ExecuteAsync(shadowsQuery, new
//         {
//             id = shadows.Select(s => s.Id).ToArray(),
//             code_name = shadows.Select(s => s.CodeName).ToArray(),
//             clan = shadows.Select(s => s.Clan).ToArray(),
//             origin = shadows.Select(s => s.Origin).ToArray(),
//             rank = shadows.Select(s => s.Rank.ToString()).ToArray()
//         });
//     }
//     
//     private async Task SeedStealthMetricsAsync(IList<StealthMetrics> stealthMetricsList, string connectionString)
//     {
//         const string stealthMetricsQuery = 
//             """
//                  INSERT INTO stealthmetrics (id, shadow_id, shadow_blend_score, silence_rating, invisibility_duration_ms, acrobatics_level)
//                  SELECT * FROM UNNEST(
//                       @id::uuid[],
//                       @shadow_id::uuid[],
//                       @shadow_blend_score::int[],
//                       @silence_rating::int[],
//                       @invisibility_duration_ms::int[],
//                       @acrobatics_level::text[]
//                  )
//             """;
//         
//         var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
//
//         await using var conn = await dataSource.OpenConnectionAsync();
//         await conn.ExecuteAsync(stealthMetricsQuery, new
//         {
//             id = stealthMetricsList.Select(s => s.Id).ToArray(),
//             shadow_id = stealthMetricsList.Select(s => s.ShadowId).ToArray(),
//             shadow_blend_score  = stealthMetricsList.Select(s => s.ShadowBlendScore).ToArray(),
//             silence_rating  = stealthMetricsList.Select(s => s.SilenceRating).ToArray(),
//             invisibility_duration_ms  = stealthMetricsList.Select(s => s.InvisibilityDurationMs).ToArray(),
//             acrobatics_level  = stealthMetricsList.Select(s => s.AcrobaticsLevel.ToString()).ToArray()
//         });
//     }
//     
//     private async Task SeedUsersAsync(IList<User> users, string connectionString)
//     {
//         const string usersQuery = 
//             """
//                  INSERT INTO users (user_id, email, hashed_password)
//                  SELECT * FROM UNNEST(
//                       @user_id::uuid[],
//                       @email::text[],
//                       @hashed_password::text[]
//                  )
//             """;
//         
//         var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
//
//         await using var conn = await dataSource.OpenConnectionAsync();
//         await conn.ExecuteAsync(usersQuery, new
//         {
//             user_id = users.Select(s => s.UserId).ToArray(),
//             email = users.Select(s => s.Email).ToArray(),
//             hashed_password  = users.Select(s => s.HashedPassword).ToArray()
//         });
//     }
//     
//     private async Task SeedTestSupportAsync(string jwtkey, string connectionString)
//     {
//         const string testSupportQuery = """
//                            INSERT INTO testsupport (jwt)
//                            VALUES (@jwt)
//                            """;
//
//         var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
//
//         await using var conn = await dataSource.OpenConnectionAsync();
//         await conn.ExecuteAsync(new CommandDefinition(testSupportQuery, new { Jwt = jwtkey }));
//     }
// }