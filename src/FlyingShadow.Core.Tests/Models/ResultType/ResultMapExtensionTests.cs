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
        Assert.Equal("Conversion on call 2 complete: 'You triggered a success on a sync call number: 1'", result.Value);
    }
    
    [Fact]
    public void Map_Success_MapsValue_Then_Bind_Continues()
    {   
        // Arrange / Act
        var result = Call(false, 1)
            .Map(message => Converter(message, 2))
            .Bind(_ => Call(false, 3));
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("You triggered a success on a sync call number: 3", result.Value);
    }

    [Fact]
    public void Map_Success_MapsValue_Then_Bind_Fails()
    {
        // Arrange / Act
        var result = Call(false, 1)
            .Map(message => Converter(message, 2))
            .Bind(_ => Call(true, 3));

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal("You triggered a failure on a sync call number: 3", result.Error);
        Assert.Null(result.Value);
    }
    
    #endregion
    #region Map - Pending Result, Sync Mapper
    #endregion
    
    #region MapAsync - Resolved Result, Async Mapper
    #endregion
    
    #region MapAsync - Pending Result, Async Mapper
    #endregion
}