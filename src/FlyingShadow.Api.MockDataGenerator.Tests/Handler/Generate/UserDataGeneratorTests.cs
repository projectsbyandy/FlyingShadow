using System.Text.Json;
using BCrypt.Net;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate;
using FlyingShadow.Api.MockDataGenerator.Handler.Generate.Internal;
using FlyingShadow.Api.MockDataGenerator.Models;
using FlyingShadow.Api.MockDataGenerator.Models.ProgressStatus;
using FlyingShadow.Api.MockDataGenerator.Utilities;
using FlyingShadow.Core.Models.Users;
using FlyingShadow.Core.Utils;
using Moq;

namespace FlyingShadow.Api.MockDataGenerator.Tests.Handler.Generate;

public class UserDataGeneratorTests : IDisposable
{
    private readonly IUserDataGenerator _userDataGenerator;
    private readonly Mock<IFileManager> _fileManagerMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ISecretGenerator> _secretGeneratorMock = new();
    private readonly string _tempDir;
    private object? _capturedPayload;
    
    public UserDataGeneratorTests()
    {
        _tempDir = Path.Combine(AppContext.BaseDirectory, "test-temp", $"{Guid.NewGuid().ToString()}");
        Directory.CreateDirectory(_tempDir);
        
        _fileManagerMock.Setup(manager => manager.ReadAsync<IList<User>>(It.IsAny<string>())).ReturnsAsync(new List<User>() 
        {   new()
            {
                UserId = Guid.Parse("e281f5cb-4188-41d5-978b-2dda37f4e245"),
                Email = "Bob@test.com"
            }, 
            new()
            {
                UserId = Guid.Parse("8749b616-ce6c-4cdc-8969-ff36f72f9eba"),
                Email = "Roy@test.com"
            }
        });
        
        _fileManagerMock
            .Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((_, payload) => _capturedPayload = payload)
            .Returns(Task.CompletedTask);
        _fileManagerMock.Setup(manager => manager.CreateDirectory(It.IsAny<string>()));
        
        _secretGeneratorMock.Setup(generator => generator.Jwt()).Returns("testJwt");
        _secretGeneratorMock.Setup(generator => generator.Password(It.IsAny<int>())).Returns("password");
        _passwordHasherMock.Setup(hasher => hasher.Hash(It.IsAny<string>())).Returns("hashedPassword");
        
        _userDataGenerator = new UserDataGenerator(_fileManagerMock.Object, _passwordHasherMock.Object, _secretGeneratorMock.Object);
    }

    [Fact]
    public async Task CredentialsAsync_Generates_JWT()
    {
        // Arrange
        var pipelineContext = CreatePipelineContext();

        // Act
        var result = await _userDataGenerator.CredentialsAsync(pipelineContext);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("testJwt", result.Value.JwtKey);
    }
    
    [Fact]
    public async Task CredentialsAsync_Generates_UserCredentials()
    {
        // Arrange
        var pipelineContext = CreatePipelineContext();

        // Act
        var result = await _userDataGenerator.CredentialsAsync(pipelineContext);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(new UserCredentials(Guid.Parse("e281f5cb-4188-41d5-978b-2dda37f4e245"), "Bob@test.com", "password", "hashedPassword"),
            result.Value.Credentials);
        Assert.Contains(new UserCredentials(Guid.Parse("8749b616-ce6c-4cdc-8969-ff36f72f9eba"), "Roy@test.com", "password", "hashedPassword"),
            result.Value.Credentials);
    }

    [Fact]
    public async Task CredentialsAsync_PasswordHasherThrowsSaltParseException_ReturnsFailure()
    {
        // Arrange
        _passwordHasherMock.Setup(hasher => hasher.Hash(It.IsAny<string>())).Throws<SaltParseException>();
        
        // Act
        var result = await _userDataGenerator.CredentialsAsync(CreatePipelineContext());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(FailureCode.Problem, result.Error);
    }
    
    [Fact]
    public async Task CredentialsAsync_FileReaderThrowsFileNotFoundException_ReturnsFailure()
    {
        // Arrange
        _fileManagerMock.Setup(manager => manager.ReadAsync<It.IsAnyType>(It.IsAny<string>()))
            .Throws<FileNotFoundException>();
        
        // Act
        var result = await _userDataGenerator.CredentialsAsync(CreatePipelineContext());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(FailureCode.Problem, result.Error);
    }
    
    [Fact]
    public async Task CredentialsAsync_NoUsersRead_ReturnsFailure()
    {
        // Arrange
        _fileManagerMock.Setup(manager => manager.ReadAsync<IList<User>>(It.IsAny<string>()))
            .ReturnsAsync(new List<User>());
        
        // Act
        var result = await _userDataGenerator.CredentialsAsync(CreatePipelineContext());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(FailureCode.Problem, result.Error);
    }

    [Fact]
    public async Task WriteJwtFileAsync_WithValidPath_WritesCorrectPayload()
    {
        // Arrange
        var defaultContext = CreatePipelineContext();
        var context = defaultContext with
        {
            MockDataOptions = defaultContext.MockDataOptions with
            {
                FakeJwtPath  = Path.Combine(_tempDir, "JwtFile.json")
            }
        };
        
        // Act
        var result = await _userDataGenerator.WriteJwtFileAsync(context);
        
        // Assert
        Assert.NotNull(_capturedPayload);
        Assert.True(result.IsSuccess);
        
        var expectedPayload = JsonSerializer.Serialize(new { jwt = new { key = context.JwtKey } });
        var actualPayload = JsonSerializer.Serialize(_capturedPayload);
        Assert.Equal(expectedPayload, actualPayload);
        
        _fileManagerMock.Verify(manager => manager.CreateDirectory(It.IsAny<string>()), Times.Once);
        _fileManagerMock.Verify(manager => manager.WriteAsync(context.MockDataOptions.FakeJwtPath, It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task WriteJwtFileAsync_FileWriterThrowsException_ReturnsFailure()
    {
        // Arrange
        _fileManagerMock.Setup(manager => manager.WriteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Throws<OperationCanceledException>();
        
        // Act
        var result = await _userDataGenerator.WriteJwtFileAsync(CreatePipelineContext());

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(FailureCode.Problem, result.Error);
    }
    
    [Fact]
    public async Task WriteLoginDetailsFileAsync_WithValidPath_WritesCorrectPayload()
    {
        // Arrange
        var defaultContext = CreatePipelineContext();
        var context = defaultContext with
        {
            MockDataOptions = defaultContext.MockDataOptions with
            {
                FakeLoginDetailsListPath  = Path.Combine(_tempDir, "LoginDetails.json")
            }
        };
        
        // Act
        var result = await _userDataGenerator.WriteLoginDetailsFileAsync(context);
        
        // Assert
        Assert.NotNull(_capturedPayload);
        Assert.True(result.IsSuccess);
        
        var expectedPayload = JsonSerializer.Serialize(new
        {
            fakeUsers = new
            {
                loginDetailsList = new[]
                {
                    new { Email = "test@test.com", Password = "password"},
                    new { Email = "test2@test.com", Password = "password2"}
                }
            }
        });
        
        var actualPayload = JsonSerializer.Serialize(_capturedPayload);
        
        Assert.Equal(expectedPayload, actualPayload);
        
        _fileManagerMock.Verify(manager => manager.CreateDirectory(It.IsAny<string>()), Times.Once);
        _fileManagerMock.Verify(manager => manager.WriteAsync(context.MockDataOptions.FakeLoginDetailsListPath, It.IsAny<object>()), Times.Once);
    }
    
    [Fact]
    public async Task WriteUsersFileAsync_WithValidPath_WritesCorrectPayloadStructure()
    {
        // Arrange
        var defaultContext = CreatePipelineContext();
        var context = defaultContext with
        {
            MockDataOptions = defaultContext.MockDataOptions with
            {
                FakeUsersPath  = Path.Combine(_tempDir, "FakeUsers.json")
            }
        };
        
        // Act
        var result = await _userDataGenerator.WriteUsersFileAsync(context);
        
        // Assert
        Assert.NotNull(_capturedPayload);
        Assert.True(result.IsSuccess);

        var expectedPayload = JsonSerializer.Serialize(new
        {
            fakeUsers = new
            {
                users = new[]
                {
                    new { UserId = Guid.Parse("850f2704-3e52-47d0-a0a0-a9ba608d620f"), Email = "test@test.com", HashedPassword = "hashedPassword" },
                    new { UserId = Guid.Parse("b10f14c4-2c3a-4fc4-b0fe-94a37b8a4afb"), Email = "test2@test.com", HashedPassword = "hashedPassword2" }
                }
            }
        });
        
        var actualPayload = JsonSerializer.Serialize(_capturedPayload);
        
        Assert.Equal(expectedPayload, actualPayload);
       
        _fileManagerMock.Verify(manager => manager.CreateDirectory(It.IsAny<string>()), Times.Once);
        _fileManagerMock.Verify(manager => manager.WriteAsync(context.MockDataOptions.FakeUsersPath, It.IsAny<object>()), Times.Once);
    }
    
    private PipelineContext CreatePipelineContext()
    {
        return new PipelineContext(new MockDataOptions()
            {
                FakeJwtPath = "unused",
                FakeShadowsPath = "unused",
                FakeStealthMetricsPath = "unused",
                FakeLoginDetailsListPath = "unused",
                FakeUsersPath = "unused"
            }, 
            JwtKey: "testJwt", Credentials: new List<UserCredentials>()
            {
                new (Guid.Parse("850f2704-3e52-47d0-a0a0-a9ba608d620f"), "test@test.com","password", "hashedPassword"),
                new (Guid.Parse("b10f14c4-2c3a-4fc4-b0fe-94a37b8a4afb"), "test2@test.com","password2", "hashedPassword2")
            });
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
}