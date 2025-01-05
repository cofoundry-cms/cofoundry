using Xunit;

namespace Cofoundry.Plugins.Azure.Tests;

[CollectionDefinition(nameof(IntegrationTestFixture))]
public class IntegrationTestFixtureCollection : ICollectionFixture<IntegrationTestFixture>
{
}
