namespace FlyingShadow.Api.Integration.Tests.Support.TestLifeCycle;

// Will only start the Flying Shadow Api once for tests tagged with collection.
[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<FlyingShadowWebAppTestFactory>
{
    public const string Name = "Integration";
}