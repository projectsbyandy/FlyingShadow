using FlyingShadow.Api.MockDataGenerator.Models;

namespace FlyingShadow.Api.MockDataGenerator.Tests.Fixtures;

public class PipelineContextFixture
{
    internal PipelineContext BuildDefaultPipelineContext()
        => new(new MockDataOptions()
            {
                FakeJwtPath = "unused",
                FakeShadowsPath = "unused",
                FakeStealthMetricsPath = "unused",
                FakeLoginDetailsListPath = "unused",
                FakeUsersPath = "unused"
            },
            JwtKey: "testJwt", Credentials: new List<UserCredentials>()
            {
                new(Guid.Parse("850f2704-3e52-47d0-a0a0-a9ba608d620f"), "test@test.com", "password", "hashedPassword"),
                new(Guid.Parse("b10f14c4-2c3a-4fc4-b0fe-94a37b8a4afb"), "test2@test.com", "password2",
                    "hashedPassword2")
            });
}