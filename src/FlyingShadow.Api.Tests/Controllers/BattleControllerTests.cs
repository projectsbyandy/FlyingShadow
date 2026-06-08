using FlyingShadow.Api.Controllers;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Services.Battle;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlyingShadow.Api.Tests.Controllers;

public class BattleControllerTests : IClassFixture<ShadowDataFixture>
{
    private readonly BattleController _sut;
    private readonly ShadowDataFixture _shadowDataFixture;
    private readonly Mock<IBattleService> _mockBattleService = new();

    public BattleControllerTests(ShadowDataFixture shadowDataFixture)
    {
        _shadowDataFixture = shadowDataFixture;
        _sut = new BattleController(_mockBattleService.Object);
    }

    [Fact]
    public async Task Battle_ServiceReturnsSuccess_Returns_Valid_BattleResponse()
    {
        // Arrange
        _mockBattleService.Setup(service => service.BattleAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<BattleResponse, Error>.Success(_shadowDataFixture.BattleResponse));
        
        // Act
        var actionResult = await _sut.BattleAsync(new BattleRequest() {
            ShadowOneCodeName = "testShadowOne", 
            ShadowTwoCodeName = "testShadowOne"
        });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var battleResponse = Assert.IsType<BattleResponse>(okResult.Value);
        Assert.Equal(battleResponse, _shadowDataFixture.BattleResponse);
    }
    
    [Fact]
    public async Task Battle_ServiceReturnsFailure_Returns_Error_BattleResponse()
    {
        // Arrange
        _mockBattleService.Setup(service => service.BattleAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync<string, string, IBattleService, Result<BattleResponse, Error>>((shadowOneCodeName, _) => Result<BattleResponse, Error>.Failure(new Error(ErrorCode.NotFound, $"Shadow: {shadowOneCodeName} not found")));
        
        const string shadowOneName = "testShadowOne";
        
        // Act
        var actionResult = await _sut.BattleAsync(new BattleRequest() {
            ShadowOneCodeName = shadowOneName, 
            ShadowTwoCodeName = "testShadowOne"
        });
        
        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var battleResponse = Assert.IsType<Error>(badResult.Value);
        Assert.Equal(ErrorCode.NotFound, battleResponse.Code);
        Assert.Equal($"Shadow: {shadowOneName} not found", battleResponse.Message);
    }
}