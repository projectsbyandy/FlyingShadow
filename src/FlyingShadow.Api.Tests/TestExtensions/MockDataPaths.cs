namespace FlyingShadow.Api.Tests.TestExtensions;

internal static class MockDataPaths
{
    public static readonly string FakeUsersPath = 
        Path.Combine(AppContext.BaseDirectory, "_GeneratedData", "fakeUsers.json");

    public static readonly string LoginDetailsListPath = 
        Path.Combine(AppContext.BaseDirectory, "_GeneratedData", "fakeLoginDetailsList.json");
}