using FlyingShadow.Api.Controllers;
using FlyingShadow.Api.Tests.Fixtures;
using FlyingShadow.Core.DTO.Authenticate;
using FlyingShadow.Core.DTO.Ninja;
using FlyingShadow.Core.Models;
using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlyingShadow.Api.Tests.Controllers;

public class FlyingShadowControllerTests : ShadowDataFixture
{
    private FlyingShadowController _sut;
    private Mock<IShadowService> _shadowServiceMock;
    private readonly IList<ShadowDto> _expectedShadowDtos;

    public FlyingShadowControllerTests()
    {
        _shadowServiceMock = new  Mock<IShadowService>();
        _sut = new FlyingShadowController(_shadowServiceMock.Object);
        _expectedShadowDtos = GetShadowDTOs();
    }
    
    [Fact]
    public void Verify_Successfully_Retrieve_All_Shadows()
    {
        // Arrange
        _shadowServiceMock.Setup(shadowService => shadowService.GetAllShadowDetails()).Returns(Result<IList<ShadowDto>, Error>.Success(_expectedShadowDtos));
        
        // Act
        var actionResult = _sut.GetShadows();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var shadowDtos = Assert.IsType<List<ShadowDto>>(okResult.Value);
        Assert.Equal(_expectedShadowDtos, shadowDtos);
    }
    
    [Fact]
    public void Verify_Problem_Retrieving_All_Shadows_Returns_500()
    {
        // Arrange
        _shadowServiceMock.Setup(shadowService => shadowService.GetAllShadowDetails()).Returns(Result<IList<ShadowDto>, Error>.Failure(new Error(ErrorCode.UnableToProcessData,
            "No Shadow Details mapped")));
        
        // Act
        var actionResult = _sut.GetShadows();
        
        // Assert
        var result = Assert.IsType<ObjectResult>(actionResult.Result);
        var error = Assert.IsType<ErrorResponse>(result.Value);
        Assert.Equal("No Shadow Details mapped", error.Message);
    }
}