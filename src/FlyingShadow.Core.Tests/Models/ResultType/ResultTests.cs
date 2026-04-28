using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Tests.Models.ResultType;

public class ResultTests
{
    [Fact]
    public void Result_WhenAccessingErrorOnSuccessResult_ThrowsException()
    {
        // Arrange / Act
        var result = () => Result<int, string>.Failure("broken");

        // Assert
        Assert.Throws<InvalidOperationException>(() => result().Value);
    }
    
    [Fact]
    public void Result_WhenAccessingValueOnFailureResult_ThrowsException()
    {
        // Arrange / Act
        var result = () => Result<int, string>.Success(1);

        // Assert
        Assert.Throws<InvalidOperationException>(() => result().Error);
    }
}