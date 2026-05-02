namespace FlyingShadow.Api.Spec.Tests.Support;

[CollectionDefinition(Name)]
public sealed class OpenApiSpecCollection : ICollectionFixture<FlyingShadowClientBuilder>, ICollectionFixture<FakeUserLoginsProvider>
{
    public const string Name = "Flying Daggers API Open API Specifications";
}