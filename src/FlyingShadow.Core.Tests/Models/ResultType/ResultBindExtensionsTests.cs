using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Tests.Fixtures;

namespace FlyingShadow.Core.Tests.Models.ResultType;

public class ResultBindExtensionsTests : ResultFixture
{
    #region Bind Resolved Result to Resolved Result
    
    [Fact]
    public void Bind_WhenAllCallsSucceed_ReturnsLastSuccessValue()
    {
        // Arrange / Act
        var result = Call(false, callNumber: 1)
            .Bind(_ => Call(false, callNumber: 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("You triggered a success on a sync call number: 2", result.Value);
    }
    
    [Fact]
    public void Bind_WhenFirstSyncCallFails_ReturnsSyncCallError()
    {
        // Arrange / Act
        var result = Call(isError: true, callNumber: 1)
            .Bind(_ => Call(isError: false, callNumber: 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("You triggered a failure on a sync call number: 1", result.Error);
    }
    
    [Fact]
    public void Bind_WhenSecondSyncCallFails_ReturnsSyncCallError()
    {
        // Arrange / Act
        var result = Call(isError: false, callNumber: 1)
            .Bind(_ => Call(isError: true, callNumber: 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("You triggered a failure on a sync call number: 2", result.Error);
    }

    [Fact]
    public void Bind_PassesValue_FromFirstCallToSecond()
    {
        // Arrange / Act
        var result = Call(false, callNumber: 1)
            .Bind(successMessage => Result<string, string>.Success($"Message from call 1: {successMessage}"));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: You triggered a success on a sync call number: 1", result.Value);
    }

    [Fact]
    public void Bind_PassesSyncSuccessValue_ToBoundFunction()
    {
        // Arrange / Act
        var result = Result<string?, string>.Success("")
            .Bind(successMessage => Result<string, string>.Success($"Message from call 1: {successMessage}"));

        // Act
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: ", result.Value);
    }
    
    #endregion

    #region Bind Pending Result to Resolved Result

    [Fact]
    public async Task Bind_WhenAsyncAndSyncCallsSucceed_ReturnsLastSuccessValue()
    {
        // Arrange / Act
        var result = await CallAsync(false, callNumber: 1)
            .Bind(_ => Call(false, callNumber: 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("You triggered a success on a sync call number: 2", result.Value);
    }

    [Fact]
    public async Task Bind_WhenFirstAsyncCallFails_ReturnsAsyncFailureError()
    {
        // Arrange / Act
        var result = await CallAsync(isError: true, callNumber: 1)
            .Bind(_ => Call(isError: false, callNumber: 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("You triggered a failure on a async call number: 1", result.Error);
    }
    
    [Fact]
    public async Task Bind_WhenSecondSyncCallFails_ReturnsSyncFailureError()
    {
        // Arrange / Act
        var result = await CallAsync(isError: false, callNumber: 1)
            .Bind(_ => Call(isError: true, callNumber: 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("You triggered a failure on a sync call number: 2", result.Error);
    }

    [Fact]
    public async Task Bind_PassesAsyncSuccessValue_ToBoundFunction()
    {
        // Arrange / Act
        var result = await CallAsync(isError: false, callNumber: 1)
            .Bind(successMessage => Result<string, string>.Success($"Message from call 1: {successMessage}"));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: You triggered a success on a async call number: 1", result.Value);
    }
    
    #endregion

    #region Bind Resolved Result to Pending Result

    [Fact]
    public async Task BindAsync_WhenSyncAndAsyncCallsSucceed_ReturnsLastSuccessValue()
    {
        // Arrange / Act
        var result = await Call(isError: false, callNumber: 1)
            .BindAsync(_ => CallAsync(isError: false, callNumber: 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("You triggered a success on a async call number: 2", result.Value);
    }
    
    [Fact]
    public async Task BindAsync_WhenFirstSyncCallFails_ReturnsSyncFailureError()
    {
        // Arrange / Act
        var result = await Call(isError: true, callNumber: 1)
            .BindAsync(_ => CallAsync(isError: false, callNumber: 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("You triggered a failure on a sync call number: 1", result.Error);
    }
    
    [Fact]
    public async Task BindAsync_WhenSecondAsyncCallFails_ReturnsAsyncFailureError()
    {
        // Arrange / Act
        var result = await Call(isError: false, callNumber: 1)
            .BindAsync(_ => CallAsync(isError: true, callNumber: 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("You triggered a failure on a async call number: 2", result.Error);
    }

    [Fact]
    public async Task BindAsync_PassesAsyncSuccessValue_ToBoundFunction()
    {
        // Arrange / Act
        var result = await Call(false, callNumber: 1)
            .BindAsync(successMessage => Task.FromResult(Result<string, string>.Success($"Message from call 1: {successMessage}")));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: You triggered a success on a sync call number: 1", result.Value);
    }
    
    #endregion

    #region Bind Pending Result to Pending Result

    [Fact]
    public async Task BindAsync_WhenAllAsyncCallsSucceed_ReturnsLastSuccessValue()
    {
        // Arrange / Act
        var result = await CallAsync(false, callNumber: 1)
            .BindAsync(_ => CallAsync(false, callNumber: 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("You triggered a success on a async call number: 2", result.Value);
    }

    [Fact]
    public async Task BindAsync_WhenFirstAsyncCallFails_ReturnsAsyncFailureError()
    {
        // Arrange / Act
        var result = await CallAsync(isError: true, callNumber: 1)
            .BindAsync(_ => CallAsync(isError: false, callNumber: 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("You triggered a failure on a async call number: 1", result.Error);
    }
    
    [Fact]
    public async Task BindAsync_AllAsyncWhenSecondCallFails_ReturnsAsyncFailureError()
    {
        // Arrange / Act
        var result = await CallAsync(isError: false, callNumber: 1)
            .BindAsync(_ => CallAsync(isError: true, callNumber: 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("You triggered a failure on a async call number: 2", result.Error);
    }
    
    [Fact]
    public async Task BindAsync_AllAsyncPassesAsyncSuccessValue_ToBoundFunction()
    {
        // Arrange / Act
        var result = await CallAsync(false, callNumber: 1)
            .BindAsync(successMessage => Task.FromResult(Result<string, string>.Success($"Message from call 1: {successMessage}")));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: You triggered a success on a async call number: 1", result.Value);
    }
    
    #endregion
}