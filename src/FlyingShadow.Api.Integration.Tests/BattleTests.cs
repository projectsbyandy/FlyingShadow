using System.Net;
using System.Net.Http.Json;
using System.Text;
using Ardalis.GuardClauses;
using FlyingShadow.Api.Integration.Tests.DTO;
using FlyingShadow.Api.Integration.Tests.Fixtures;
using FlyingShadow.Api.Integration.Tests.Support.TestExtensions;
using FlyingShadow.Api.Integration.Tests.Support.TestLifeCycle;
using FlyingShadow.Api.Utils;
using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.Models.Battle;
using FlyingShadow.Core.Models.Ninja;

namespace FlyingShadow.Api.Integration.Tests;

[Collection(IntegrationTestCollection.Name)]
public class BattleTests : AuthenticationFixture
{
    private readonly HttpClient _client;
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

    public BattleTests(FlyingShadowWebAppTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [JsonMockDataFact]
    public async Task Verify_AuthenticatedCall_ReturnsBattleResponse()
    {
        // Arrange
        IList<Shadow> shadows = ConfigReader.GetConfigurationSection<List<Shadow>>("FakeShadows");
        var token = await GetAuthTokenAsync(_client, _cancellationToken);
        AddAuthHeader(_client, token);
        
        var battleRequest = new BattleRequest(shadows.First().CodeName, shadows.Last().CodeName);
        
        // Act
        var response = await _client.PostAsJsonAsync("api/Battle/Start", battleRequest, _cancellationToken);
        var battleReport =  await response.Content.ReadFromJsonAsync<BattleReport>(_cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(battleReport);
        Assert.Equal("Silent Serpent Wins!", battleReport.Outcome);
        Assert.Equal("Shadow Wolf II", battleReport.ShadowOneStats.CodeName);
        Assert.Equal(2968m, battleReport.ShadowOneStats.CombatPower);
        Assert.Equal(3.667m, battleReport.ShadowOneStats.EvasionIndex);
        Assert.Equal(40.5m, battleReport.ShadowOneStats.StealthScore);
        Assert.Equal(1198.6m, battleReport.ShadowOneStats.OverallRating);
        Assert.Equal("Silent Serpent", battleReport.ShadowTwoStats.CodeName);
        Assert.Equal(3312m, battleReport.ShadowTwoStats.CombatPower);
        Assert.Equal(3.932m, battleReport.ShadowTwoStats.EvasionIndex);
        Assert.Equal(64m, battleReport.ShadowTwoStats.StealthScore);
        Assert.Equal(1342.17m, battleReport.ShadowTwoStats.OverallRating);
        Assert.Equal("Silent Serpent", battleReport.StatBreakdown.CombatPowerWinner);
        Assert.Equal("Silent Serpent", battleReport.StatBreakdown.EvasionIndexWinner);
        Assert.Equal("Silent Serpent", battleReport.StatBreakdown.StealthScoreWinner);
    }

    [JsonMockDataTheory]
    [InlineData("", new[] {"", "request"} )]
    [InlineData("{}", new[] {"ShadowOneCodeName", "ShadowTwoCodeName"} )]
    [InlineData("{\"shadowOneCodeName\":\"test1\"}", new[] {"ShadowTwoCodeName" } )]
    [InlineData("{\"shadowTwoCodeName\":\"test2\"}", new[] {"ShadowOneCodeName" } )]
    public async Task Verify_AuthenticatedCallToBattleEndpointWithMissingFields_ReturnsBadRequest(string requestBodyContent, string[] expectedMissingFieldsInError)
    {
        // Arrange
        var token = await GetAuthTokenAsync(_client, _cancellationToken);
        AddAuthHeader(_client, token);
        var content = new StringContent(requestBodyContent, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("api/Battle/Start", content, _cancellationToken);
        var validationResponse = await response.Content.ReadFromJsonAsync<ValidationResponse>(_cancellationToken);
        var errors = Guard.Against.Null(validationResponse).Errors.Select(error => error.Key).ToList();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(expectedMissingFieldsInError.Order().ToList(), errors.Order());
    }
    
    [JsonMockDataFact]
    public async Task Verify_UnauthenticatedCall_ReturnsUnauthorized()
    {
        // Arrange / Act
        var response = await _client.PostAsync("api/Battle/Start", new StringContent(""), _cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}