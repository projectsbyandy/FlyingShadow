using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Tests.Fixtures;

public abstract class ResultFixture
{
    protected int ConvertedMethodCalled;
    protected int ConverterAsyncMethodCalled;
    
    protected static Result<string, string> Call(bool isError, int callNumber)
    {
        return isError
            ? Result<string, string>.Failure($"You triggered a failure on a sync call number: {callNumber}")
            : Result<string, string>.Success($"You triggered a success on a sync call number: {callNumber}");
    }

    protected static Task<Result<string, string>> CallAsync(bool isError, int callNumber)
    {
        var result = isError
            ? Result<string, string>.Failure($"You triggered a failure on a async call number: {callNumber}")
            : Result<string, string>.Success($"You triggered a success on a async call number: {callNumber}");
        
        return Task.FromResult(result);
    }
    
    protected string Converter(string converter, int callNumber)
    {
        ConvertedMethodCalled++;
        return $"Conversion on call {callNumber} complete: '{converter}'";
    }
    
    protected Task<string> ConverterAsync(string converter, int callNumber)
    {
        ConverterAsyncMethodCalled++;
        return Task.FromResult($"Async Conversion on call {callNumber} complete: '{converter}'");
    }
}