using Cofoundry.Core;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class IsPagePathUniqueQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "IsPagePathUnqQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public IsPagePathUniqueQueryHandlerTests(
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
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var isUnique = await contentRepository
                .Pages()
                .IsPathUnique(new IsPagePathUniqueQuery()
                {
                    PageDirectoryId = directoryId,
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
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var isUnique = await contentRepository
                .Pages()
                .IsPathUnique(new IsPagePathUniqueQuery()
                {
                    PageDirectoryId = directoryId,
                    UrlPath = uniqueData
                })
                .ExecuteAsync();

            isUnique.Should().BeFalse();
        }

        [Fact]
        public async Task WhenExistingPageAndPathNotUnique_ReturnsFalse()
        {
            var uniqueData = SlugFormatter.ToSlug(UNIQUE_PREFIX + "ExPagNotUniq_RetFalse");

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);
            var page2Path = uniqueData + "a";
            await app.TestData.Pages().AddAsync(page2Path, directoryId);

            var isUnique = await contentRepository
                .Pages()
                .IsPathUnique(new IsPagePathUniqueQuery()
                {
                    PageId = pageId,
                    PageDirectoryId = directoryId,
                    UrlPath = page2Path
                })
                .ExecuteAsync();

            isUnique.Should().BeFalse();
        }

        [Fact]
        public async Task WhenExistingPageAndUnique_ReturnsTrue()
        {
            var uniqueData = SlugFormatter.ToSlug(UNIQUE_PREFIX + "ExPagUniq_RetTrue");

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);


            var isUnique = await contentRepository
                .Pages()
                .IsPathUnique(new IsPagePathUniqueQuery()
                {
                    PageId = pageId,
                    PageDirectoryId = directoryId,
                    UrlPath = uniqueData + "a"
                })
                .ExecuteAsync();

            isUnique.Should().BeTrue();
        }

        [Fact]
        public async Task WhenExistingPagUnchanged_ReturnsTrue()
        {
            var uniqueData = SlugFormatter.ToSlug(UNIQUE_PREFIX + "ExPagUnchanged_RetTrue");

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);


            var isUnique = await contentRepository
                .Pages()
                .IsPathUnique(new IsPagePathUniqueQuery()
                {
                    PageId = pageId,
                    PageDirectoryId = directoryId,
                    UrlPath = uniqueData
                })
                .ExecuteAsync();

            isUnique.Should().BeTrue();
        }
    }
}
