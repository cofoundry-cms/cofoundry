using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class IsPageDirectoryPathUniqueQueryHandlerTests
    {
        const string DIRECTORY_PREFIX = "IsPageDirPathUnqCHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public IsPageDirectoryPathUniqueQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        public async Task WhenPathUnique_ReturnsTrue()
        {
            var uniqueData = DIRECTORY_PREFIX + nameof(WhenPathUnique_ReturnsTrue);
            var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
            var uniqueData = DIRECTORY_PREFIX + nameof(WhenPathNotUnique_ReturnsFalse);
            var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
            var uniqueData = DIRECTORY_PREFIX + nameof(WhenExistingDirectory_ReturnsFalse);
            var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var childDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData, parentDirectoryId);
            await _testDataHelper.PageDirectories().AddAsync(uniqueData + "a", parentDirectoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
