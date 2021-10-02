using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
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

        public async Task WhenPathUnique_ReturnsTrue()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPathUnique_ReturnsTrue);

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

        public async Task WhenPathNotUnique_ReturnsFalse()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPathNotUnique_ReturnsFalse);

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

            isUnique.Should().BeFalse();
        }

        public async Task WhenExistingDirectory_ReturnsFalse()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenExistingDirectory_ReturnsFalse);

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
                    UrlPath = uniqueData,
                    PageDirectoryId = childDirectoryId
                })
                .ExecuteAsync();

            isUnique.Should().BeFalse();
        }
    }
}
