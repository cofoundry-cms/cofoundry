namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class IsPageDirectoryPathUniqueQueryHandlerTests
{
    const string UNIQUE_PREFIX = "IsPageDirPathUnqQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public IsPageDirectoryPathUniqueQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task WhenPathUnique_ReturnsTrue()
    {
        var uniqueData = SlugFormatter.ToSlug(UNIQUE_PREFIX + nameof(WhenPathUnique_ReturnsTrue));

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var isUnique = await contentRepository
            .PageDirectories()
            .IsPathUnique(new IsPageDirectoryPathUniqueQuery()
            {
                ParentPageDirectoryId = parentDirectoryId,
                UrlPath = uniqueData + "a"
            })
            .ExecuteAsync();

        isUnique.Should().BeTrue();
    }

    [Fact]
    public async Task WhenPathNotUnique_ReturnsFalse()
    {
        var uniqueData = SlugFormatter.ToSlug(UNIQUE_PREFIX + nameof(WhenPathNotUnique_ReturnsFalse));

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var childDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData, parentDirectoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var isUnique = await contentRepository
            .PageDirectories()
            .IsPathUnique(new IsPageDirectoryPathUniqueQuery()
            {
                ParentPageDirectoryId = parentDirectoryId,
                UrlPath = uniqueData
            })
            .ExecuteAsync();

        isUnique.Should().BeFalse();
    }

    [Fact]
    public async Task WhenExistingDirectoryAndPathNotUnique_ReturnsFalse()
    {
        var uniqueData = SlugFormatter.ToSlug(UNIQUE_PREFIX + "ExDirNotUniq_RetFalse");

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var childDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData, parentDirectoryId);
        var directory2Patg = uniqueData + "a";
        await app.TestData.PageDirectories().AddAsync(directory2Patg, parentDirectoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var isUnique = await contentRepository
            .PageDirectories()
            .IsPathUnique(new IsPageDirectoryPathUniqueQuery()
            {
                ParentPageDirectoryId = parentDirectoryId,
                UrlPath = directory2Patg,
                PageDirectoryId = childDirectoryId
            })
            .ExecuteAsync();

        isUnique.Should().BeFalse();
    }

    [Fact]
    public async Task WhenExistingDirectoryAndUnique_ReturnsTrue()
    {
        var uniqueData = SlugFormatter.ToSlug(UNIQUE_PREFIX + "ExDirUniq_RetTrue");

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var childDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData, parentDirectoryId);
        await app.TestData.PageDirectories().AddAsync(uniqueData + "a", parentDirectoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var isUnique = await contentRepository
            .PageDirectories()
            .IsPathUnique(new IsPageDirectoryPathUniqueQuery()
            {
                ParentPageDirectoryId = parentDirectoryId,
                UrlPath = uniqueData + "b",
                PageDirectoryId = childDirectoryId
            })
            .ExecuteAsync();

        isUnique.Should().BeTrue();
    }

    [Fact]
    public async Task WhenExistingDirectoryUnchanged_ReturnsTrue()
    {
        var uniqueData = SlugFormatter.ToSlug(UNIQUE_PREFIX + "ExDirUnchanged_RetTrue");

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var childDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData, parentDirectoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var isUnique = await contentRepository
            .PageDirectories()
            .IsPathUnique(new IsPageDirectoryPathUniqueQuery()
            {
                ParentPageDirectoryId = parentDirectoryId,
                UrlPath = uniqueData,
                PageDirectoryId = childDirectoryId
            })
            .ExecuteAsync();

        isUnique.Should().BeTrue();
    }
}
