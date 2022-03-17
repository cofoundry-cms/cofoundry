namespace Cofoundry.Web.Tests.Integration;

/// <summary>
/// A collection that is scoped for the test session to ensure that the
/// database is only initialized and seeded the once. Database setup is
/// done at the start of the test session and the application and web 
/// client factories are disposed of at the end of the session.
/// </summary>
[CollectionDefinition(nameof(DbDependentTestApplicationFactory))]
public class WebDbDependentTestApplicationFactoryCollection
    : ICollectionFixture<DbDependentTestApplicationFactory>
    , IClassFixture<TestWebApplicationFactory>
{
}
