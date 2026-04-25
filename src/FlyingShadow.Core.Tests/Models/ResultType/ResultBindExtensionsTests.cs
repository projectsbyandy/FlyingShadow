using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Tests.Fixtures;

namespace FlyingShadow.Core.Tests.Models.ResultType;

public class ResultBindExtensionsTests : ResultFixture
{
    #region Bind Resolved Result to Resolved Result
    
    [Fact]
    public void Bind_Chains_Resolved_Result_To_Resolved_Result_With_Success_Outcome()
    {
        // Arrange / Act
        var result = Call(false, 1)
            .Bind(_ => Call(false, 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("You triggered a success on a sync call number: 2", result.Value);
        Assert.Null(result.Error);
    }
    
    [Theory]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 2)]
    public void Bind_Chains_Resolved_Result_To_Resolved_Result_With_Failure_Outcome(bool callOneReturnsError, bool callTwoReturnsError, int expectedErrorCallNumber)
    {
        // Arrange / Act
        var result = Call(callOneReturnsError, 1)
            .Bind(_ => Call(callTwoReturnsError, 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"You triggered a failure on a sync call number: {expectedErrorCallNumber}", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Bind_Passes_Value_From_First_Call_To_Second()
    {
        // Arrange / Act
        var result = Call(false, 1)
            .Bind(successMessage => Result<string, string>.Success($"Message from call 1: {successMessage}"));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: You triggered a success on a sync call number: 1", result.Value);
    }

    [Fact]
    public void Bind_Passes_Empty_From_First_Call_To_Second()
    {
        // Arrange / Act
        var result = Result<string?, string>.Success("")
            .Bind(successMessage => Result<string, string>.Success($"Message from call 1: {successMessage}"));

        // Act
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: ", result.Value);
    }
    
    [Fact]
    public void Bind_ResultSuccessNullValue_Throws_Null_Exception()
    {
        // Arrange / Act
        var result = () => Result<string?, string>.Success(null);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(result);
        Assert.Equal("value", exception.ParamName);
    }
    
    [Fact]
    public void Bind_ResultFailureNullValue_Throws_Null_Exception()
    {
        // Arrange / Act
        var result = () => Result<string?, string>.Failure(null!);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(result);
        Assert.Equal("error", exception.ParamName);
    }
    
    #endregion

    #region Bind Pending Result to Resolved Result

    [Fact]
    public async Task Bind_Chains_Sync_To_Pending_Async_Task_With_Success_Outcome()
    {
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .Bind(_ => Call(false, 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("You triggered a success on a sync call number: 2", result.Value);
        Assert.Null(result.Error);
    }
    
    [Theory]
    [InlineData(true, false, 1, "async")]
    [InlineData(false, true, 2, "sync")]
    public async Task Bind_Chains_Sync_To_Pending_Async_Task_With_Failure_Outcome(bool callOneReturnsError, bool callTwoReturnsError, int expectedErrorCallNumber, string execution)
    {
        // Arrange / Act
        var result = await CallAsync(callOneReturnsError, 1)
            .Bind(_ => Call(callTwoReturnsError, 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"You triggered a failure on a {execution} call number: {expectedErrorCallNumber}", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Bind_To_Pending_Result_Passes_Value_From_First_Call_To_Second()
    {
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .Bind(successMessage => Result<string, string>.Success($"Message from call 1: {successMessage}"));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: You triggered a success on a async call number: 1", result.Value);
    }
    
    #endregion

    #region Bind Resolved Result to Pending Result

    [Fact]
    public async Task BindAsync_Chains_To_Resolved_Result_With_Success_Outcome()
    {
        // Arrange / Act
        var result = await Call(false, 1)
            .BindAsync(_ => CallAsync(false, 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("You triggered a success on a async call number: 2", result.Value);
        Assert.Null(result.Error);
    }
    
    [Theory]
    [InlineData(true, false, 1, "sync")]
    [InlineData(false, true, 2, "async")]
    public async Task BindAsync_Chains_To_Resolved_Result_With_Failure_Outcome(bool callOneReturnsError, bool callTwoReturnsError, int expectedErrorCallNumber, string execution)
    {
        // Arrange / Act
        var result = await Call(callOneReturnsError, 1)
            .BindAsync(_ => CallAsync(callTwoReturnsError, 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"You triggered a failure on a {execution} call number: {expectedErrorCallNumber}", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task BindAsync_To_Resolved_Result_Passes_Value_From_First_Call_To_Second()
    {
        // Arrange / Act
        var result = await Call(false, 1)
            .BindAsync(successMessage => Task.FromResult(Result<string, string>.Success($"Message from call 1: {successMessage}")));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: You triggered a success on a sync call number: 1", result.Value);
    }
    
    #endregion

    #region Bind Pending Result to Pending Result

    [Fact]
    public async Task BindAsync_Chains_Async_To_Pending_Result_With_Success_Outcome()
    {
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .BindAsync(_ => CallAsync(false, 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("You triggered a success on a async call number: 2", result.Value);
        Assert.Null(result.Error);
    }
    
    [Theory]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 2)]
    public async Task BindAsync_Chains_Async_To_Pending_Result_With_Failure_Outcome(bool callOneReturnsError, bool callTwoReturnsError, int expectedErrorCallNumber)
    {
        // Arrange / Act
        var result = await CallAsync(callOneReturnsError, 1)
            .BindAsync(_ => CallAsync(callTwoReturnsError, 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"You triggered a failure on a async call number: {expectedErrorCallNumber}", result.Error);
        Assert.Null(result.Value);
    }
    
    [Fact]
    public async Task BindAsync_To_Pending_Result_Passes_Value_From_First_Call_To_Second()
    {
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .BindAsync(successMessage => Task.FromResult(Result<string, string>.Success($"Message from call 1: {successMessage}")));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Message from call 1: You triggered a success on a async call number: 1", result.Value);
    }
    
    #endregion
}