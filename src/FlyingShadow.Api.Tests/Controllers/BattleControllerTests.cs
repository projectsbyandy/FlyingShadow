using FlyingShadow.Api.Controllers;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.DTO.Battle;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Services.Battle;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlyingShadow.Api.Tests.Controllers;

public class BattleControllerTests : ShadowDataFixture
{
    private readonly BattleController _sut;
    private readonly Mock<IBattleService> _mockBattleService = new();

    public BattleControllerTests()
    {
        _sut = new BattleController(_mockBattleService.Object);
    }

    [Fact]
    public void Battle_ServiceReturnsSuccess_Returns_Valid_BattleResponse()
    {
        // Arrange
        _mockBattleService.Setup(service => service.Battle(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Result<BattleResponse, Error>.Success(BattleResponse));
        
        // Act
        var actionResult = _sut.Battle(new BattleRequest("testShadowOne", "testShadowOne"));

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var battleResponse = Assert.IsType<BattleResponse>(okResult.Value);
        Assert.Equal(battleResponse, BattleResponse);
    }
    
    [Fact]
    public void Battle_ServiceReturnsFailure_Returns_Error_BattleResponse()
    {
        // Arrange
        _mockBattleService.Setup(service => service.Battle(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((shadowOneCodeName, _) => Result<BattleResponse, Error>.Failure(new Error(ErrorCode.NotFound, $"Shadow: {shadowOneCodeName} not found")));
        
        const string shadowOneName = "testShadowOne";
        
        // Act
        var actionResult = _sut.Battle(new BattleRequest(shadowOneName, "testShadowOne"));

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var battleResponse = Assert.IsType<Error>(badResult.Value);
        Assert.Equal(ErrorCode.NotFound, battleResponse.Code);
        Assert.Equal($"Shadow: {shadowOneName} not found", battleResponse.Message);
    }
}