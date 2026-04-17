using FlyingShadow.Core.Models.ResultType;

namespace FlyingShadow.Core.Tests.Fixtures;

public class ResultFixture
{
    // Warnings due to async and sync versions of method for testing.
    #pragma warning disable 
    protected Result<string, string> Call(bool isError, int callNumber)
    {
        return isError
            ? Result<string, string>.Failure($"You triggered a failure on a sync call number: {callNumber}")
            : Result<string, string>.Success($"You triggered a success on a sync call number: {callNumber}");
    }

    protected Task<Result<string, string>> CallAsync(bool isError, int callNumber)
    {
        var result = isError
            ? Result<string, string>.Failure($"You triggered a failure on a async call number: {callNumber}")
            : Result<string, string>.Success($"You triggered a success on a async call number: {callNumber}");
        
        return Task.FromResult(result);
    }
    
    protected string Converter(string converter, int callNumber)
    {
        return $"Conversion on call {callNumber} complete: '{converter}'";
    }
    
    protected Task<string> ConverterAsync(string converter, int callNumber)
    {
        return Task.FromResult($"Conversion on call {callNumber} complete: '{converter}'");
    }
}