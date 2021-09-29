using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class IsPagePathUniqueQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "IsPagePathUnqQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public IsPagePathUniqueQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        public async Task WhenPathUnique_ReturnsTrue()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPathUnique_ReturnsTrue);

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
