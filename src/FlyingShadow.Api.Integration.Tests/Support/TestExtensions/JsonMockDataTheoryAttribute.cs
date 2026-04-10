namespace FlyingShadow.Api.Integration.Tests.Support.TestExtensions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class JsonMockDataTheoryAttribute : TheoryAttribute
{
    public JsonMockDataTheoryAttribute()
    {
        if (!File.Exists(MockDataPaths.FakeUsersPath) || !File.Exists(MockDataPaths.LoginDetailsListPath))
            Skip = "Mock data files not present. Run: dotnet build -p:GenerateMockData=true";
    }
}