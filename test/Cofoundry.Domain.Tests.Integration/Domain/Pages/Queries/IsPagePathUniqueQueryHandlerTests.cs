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

        public async Task WhenPathUnique_ReturnsTrue()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPathUnique_ReturnsTrue);

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

        public async Task WhenPathNotUnique_ReturnsFalse()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPathNotUnique_ReturnsFalse);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
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

        public async Task WhenExistingPage_ReturnsFalse()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenExistingPage_ReturnsFalse);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var isUnique = await contentRepository
                .Pages()
                .IsPathUnique(new IsPagePathUniqueQuery()
                {
                    PageId = pageId,
                    PageDirectoryId = directoryId,
                    UrlPath = uniqueData
                })
                .ExecuteAsync();

            isUnique.Should().BeFalse();
        }
    }
}
