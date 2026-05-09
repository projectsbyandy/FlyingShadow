using FlyingShadow.Api.MockDataGenerator.Handler;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate;
using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;
using FlyingShadow.Api.MockDataGenerator.Tests.Fixtures;
using FlyingShadow.Core.Models.ResultType;
using Moq;

namespace FlyingShadow.Api.MockDataGenerator.Tests.Handler;

public class MockDataHandlerTests : IClassFixture<PipelineContextFixture>
{
    private readonly MockDataHandler _sut;
    private readonly Mock<IPreReqValidator> _preReqValidatorMock = new();
    private readonly Mock<IUserDataGenerator> _userDataGeneratorMock = new();
    private readonly Mock<IShadowDataCopier> _shadowDataCopierMock = new();
    private readonly PipelineContextFixture _pipelineContextFixture;


    public MockDataHandlerTests(PipelineContextFixture pipelineContextFixture)
    {
        _pipelineContextFixture = pipelineContextFixture;
        SetupMocksWithSuccessScenario();
        _sut = new MockDataHandler(_preReqValidatorMock.Object, _userDataGeneratorMock.Object, _shadowDataCopierMock.Object);
    }


    [Fact]
    public async Task Process_WithNoHandlerErrors_ReturnsSuccessfulExitCode0()
    {
        // Arrange / Act
        var result = await _sut.Process();
        
        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public async Task Process_WithCheckFilesExistWarning_ReturnsFailureCode0()
    {
        // Arrange
        _preReqValidatorMock.Setup(validator => validator.CheckFilesExistAsync())
            .ReturnsAsync(Result<PipelineContext, FailureCode>.Failure(FailureCode.Warning));
        
        // Act
        var result = await _sut.Process();
        
        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public async Task Process_WithErrorGeneratingUserDetails_ReturnsFailureCode1()
    {
        // Arrange
        _userDataGeneratorMock.Setup(generator => generator.WriteLoginDetailsFileAsync(It.IsAny<PipelineContext>()))
            .ReturnsAsync(Result<PipelineContext, FailureCode>.Failure(FailureCode.Problem));
        
        // Act
        var result = await _sut.Process();
        
        // Assert
        Assert.Equal(1, result);
    }

    private void SetupMocksWithSuccessScenario()
    {
        var context = _pipelineContextFixture.BuildDefaultPipelineContext();
        var success = Result<PipelineContext, FailureCode>.Success(context);

        _preReqValidatorMock
            .Setup(v => v.CheckFilesExistAsync())
            .ReturnsAsync(success);

        _userDataGeneratorMock
            .Setup(g => g.CredentialsAsync(It.IsAny<PipelineContext>()))
            .ReturnsAsync(success);

        _userDataGeneratorMock
            .Setup(g => g.WriteJwtFileAsync(It.IsAny<PipelineContext>()))
            .ReturnsAsync(success);

        _userDataGeneratorMock
            .Setup(g => g.WriteLoginDetailsFileAsync(It.IsAny<PipelineContext>()))
            .ReturnsAsync(success);

        _userDataGeneratorMock
            .Setup(g => g.WriteUsersFileAsync(It.IsAny<PipelineContext>()))
            .ReturnsAsync(success);

        _shadowDataCopierMock
            .Setup(c => c.Process(It.IsAny<PipelineContext>()))
            .Returns(Result<SuccessCode, FailureCode>.Success(0));
    }
}