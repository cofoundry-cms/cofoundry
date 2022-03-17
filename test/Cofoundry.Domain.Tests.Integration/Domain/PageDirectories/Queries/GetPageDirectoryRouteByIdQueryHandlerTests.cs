namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class GetPageDirectoryRouteByIdQueryHandlerTests
{
    const string UNIQUE_PREFIX = "GAllPageDirRouteByIdQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public GetPageDirectoryRouteByIdQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task ReturnsMappedRoute()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(ReturnsMappedRoute);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var parentDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);
        var parentDirectoryId = await contentRepository
            .PageDirectories()
            .AddAsync(parentDirectoryCommand);

        var directoryId = await app.TestData.PageDirectories().AddAsync("Dir-1", parentDirectoryId);

        var directory = await contentRepository
            .PageDirectories()
            .GetById(directoryId)
            .AsRoute()
            .ExecuteAsync();

        var parentFullPath = "/" + parentDirectoryCommand.UrlPath;

        using (new AssertionScope())
        {

            directory.Should().NotBeNull();
            directory.IsSiteRoot().Should().BeFalse();
            directory.LocaleVariations.Should().NotBeNull().And.BeEmpty();
            directory.Name.Should().Be("Dir-1");
            directory.ParentPageDirectoryId.Should().Be(parentDirectoryId);
            directory.FullUrlPath.Should().Be(parentFullPath + "/dir-1");
            directory.UrlPath.Should().Be("dir-1");
        }
    }
}
