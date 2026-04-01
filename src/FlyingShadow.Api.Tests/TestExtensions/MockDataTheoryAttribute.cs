namespace FlyingShadow.Api.Tests.TestExtensions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class MockDataTheoryAttribute : TheoryAttribute
{
    public MockDataTheoryAttribute()
    {
        if (!File.Exists(MockDataPaths.FakeUsersPath) || !File.Exists(MockDataPaths.LoginDetailsListPath))
            Skip = "Mock data files not present. Run: dotnet build -p:GenerateMockData=true";
    }
}