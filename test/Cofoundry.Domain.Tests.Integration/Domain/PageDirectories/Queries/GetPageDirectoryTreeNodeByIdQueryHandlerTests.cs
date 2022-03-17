namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class GetPageDirectoryTreeNodeByIdQueryHandlerTests
{
    const string UNIQUE_PREFIX = "GPageDirTreeNodeByIdQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public GetPageDirectoryTreeNodeByIdQueryHandlerTests(
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
        await app.TestData.PageDirectories().AddAsync("Dir-1-A", directoryId);
        await app.TestData.PageDirectories().AddAsync("Dir-1-B", directoryId);
        await app.TestData.PageDirectories().AddAsync("Dir-1-C", directoryId);

        var directory = await contentRepository
            .PageDirectories()
            .GetById(directoryId)
            .AsNode()
            .ExecuteAsync();

        var parentFullPath = "/" + parentDirectoryCommand.UrlPath;

        using (new AssertionScope())
        {
            directory.Should().NotBeNull();
            directory.AuditData.Should().NotBeNull();
            directory.AuditData.Creator.Should().NotBeNull();
            directory.AuditData.CreateDate.Should().NotBeDefault();
            directory.ChildPageDirectories.Should().NotBeNull().And.HaveCount(3);
            directory.Depth.Should().Be(2);
            directory.Name.Should().Be("Dir-1");
            directory.UrlPath.Should().Be("dir-1");
            directory.FullUrlPath.Should().Be(parentFullPath + "/dir-1");
            directory.ParentPageDirectoryId.Should().Be(parentDirectoryId);
            directory.ParentPageDirectory.Should().NotBeNull();
            directory.ParentPageDirectory.FullUrlPath.Should().Be(parentFullPath);
        }
    }
}
