using FlyingShadow.Core.Models.ResultType;
using FlyingShadow.Core.Tests.Fixtures;

namespace FlyingShadow.Core.Tests.Models.ResultType;

public class ResultMapExtensionTests : ResultFixture
{
    #region Map - Resolved Result, Sync Mapper
    
    [Fact]
    public void Map_Success_MapsValue()
    {
        // Arrange / Act
        var result = Call(false, 1)
            .Map(message => Converter(message, 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, ConvertedMethodCalled);
        Assert.Equal("Conversion on call 2 complete: 'You triggered a success on a sync call number: 1'", result.Value);
    }
    
    [Fact]
    public void Map_SuccessMapsValue_ThenBindContinues()
    {   
        // Arrange / Act
        var result = Call(false, 1)
            .Map(message => Converter(message, 2))
            .Bind(_ => Call(false, 3));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, ConvertedMethodCalled);
        Assert.Equal("You triggered a success on a sync call number: 3", result.Value);
    }

    [Fact]
    public void Map_SuccessMapsValue_ThenBindFails()
    {
        // Arrange / Act
        var result = Call(false, 1)
            .Map(message => Converter(message, 2))
            .Bind(_ => Call(true, 3));

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(1, ConvertedMethodCalled);
        Assert.Equal("You triggered a failure on a sync call number: 3", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Map_EarlierResolvedResultFailsBeforeMap_ThenMapperNotInvoked()
    {
        // Arrange / Act
        var result = Call(true, 1)
            .Map(message => Converter(message, 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(0, ConvertedMethodCalled);
        Assert.Equal("You triggered a failure on a sync call number: 1", result.Error);
    }
    
    #endregion
    #region Map - Pending Result, Sync Mapper
    
    [Fact]
    public async Task Map_PendingSuccess_MapsValue()
    {
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .Map(message => Converter(message, 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, ConvertedMethodCalled);
        Assert.Equal("Conversion on call 2 complete: 'You triggered a success on a async call number: 1'", result.Value);
    }
    
    [Fact]
    public async Task Map_PendingSuccessMapsValue_ThenBindContinues()
    {   
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .Map(message => Converter(message, 2))
            .Bind(_ => Call(false, 3));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, ConvertedMethodCalled);
        Assert.Equal("You triggered a success on a sync call number: 3", result.Value);
    }

    [Fact]
    public async Task Map_PendingSuccessMapsValue_ThenBindFails()
    {
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .Map(message => Converter(message, 2))
            .Bind(_ => Call(true, 3));

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(1, ConvertedMethodCalled);
        Assert.Equal("You triggered a failure on a sync call number: 3", result.Error);
        Assert.Null(result.Value);
    }
    
    [Fact]
    public async Task Map_EarlierPendingResultFailsBeforeMap_ThenMapperNotInvoked()
    {
        // Arrange / Act
        var result = await CallAsync(true, 1)
            .Map(message => Converter(message, 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(0, ConvertedMethodCalled);
        Assert.Equal("You triggered a failure on a async call number: 1", result.Error);
    }
    
    #endregion
    #region MapAsync - Resolved Result, Async Mapper
    
    [Fact]
    public async Task MapAsync_Success_AsyncMapsValue()
    {
        // Arrange / Act
        var result = await Call(false, 1)
            .MapAsync(message => ConverterAsync(message, 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, ConverterAsyncMethodCalled);
        Assert.Equal("Async Conversion on call 2 complete: 'You triggered a success on a sync call number: 1'", result.Value);
    }
    
    [Fact]
    public async Task MapAsync_Success_AsyncMapsValue_ThenBindContinues()
    {   
        // Arrange / Act
        var result = await Call(false, 1)
            .MapAsync(message => ConverterAsync(message, 2))
            .Bind(_ => Call(false, 3));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, ConverterAsyncMethodCalled);
        Assert.Equal("You triggered a success on a sync call number: 3", result.Value);
    }

    [Fact]
    public async Task MapAsync_SuccessAsyncMapsValue_ThenBindFails()
    {
        // Arrange / Act
        var result = await Call(false, 1)
            .MapAsync(message => ConverterAsync(message, 2))
            .Bind(_ => Call(true, 3));

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(1, ConverterAsyncMethodCalled);
        Assert.Equal("You triggered a failure on a sync call number: 3", result.Error);
        Assert.Null(result.Value);
    }
    
    [Fact]
    public async Task MapAsync_EarlierResolvedResultFailsBeforeMap_ThenMapperNotInvoked()
    {
        // Arrange / Act
        var result = await Call(true, 1)
            .MapAsync(message => ConverterAsync(message, 2));
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(0, ConverterAsyncMethodCalled);
        Assert.Equal("You triggered a failure on a sync call number: 1", result.Error);
        Assert.Null(result.Value);
    }
    
    #endregion
    #region MapAsync - Pending Result, Async Mapper
    
    [Fact]
    public async Task MapAsync_PendingSuccess_AsyncMapsValue()
    {
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .MapAsync(message => ConverterAsync(message, 2));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, ConverterAsyncMethodCalled);
        Assert.Equal("Async Conversion on call 2 complete: 'You triggered a success on a async call number: 1'", result.Value);
    }
    
    [Fact]
    public async Task MapAsync_PendingSuccessMapsValue_ThenBindContinues()
    {   
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .MapAsync(message => ConverterAsync(message, 2))
            .Bind(_ => Call(false, 3));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, ConverterAsyncMethodCalled);
        Assert.Equal("You triggered a success on a sync call number: 3", result.Value);
    }
    
    [Fact]
    public async Task MapAsync_PendingSuccessAsyncMapsValue_ThenBindFails()
    {
        // Arrange / Act
        var result = await CallAsync(false, 1)
            .MapAsync(message => ConverterAsync(message, 2))
            .Bind(_ => Call(true, 3));

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(1, ConverterAsyncMethodCalled);
        Assert.Equal("You triggered a failure on a sync call number: 3", result.Error);
        Assert.Null(result.Value);
    }
    
    [Fact]
    public async Task MapAsync_EarlierPendingResultFailsBeforeMap_ThenMapperNotInvoked()
    {
        // Arrange / Act
        var result = await CallAsync(true, 1)
            .MapAsync(message => ConverterAsync(message, 2));
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(0, ConverterAsyncMethodCalled);
        Assert.Equal("You triggered a failure on a async call number: 1", result.Error);
    }
    #endregion
}