using Cofoundry.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageDirectoryRouteByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GAllPageDirRouteByIdQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageDirectoryRouteByIdQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task ReturnsMappedRoute()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsMappedRoute);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var parentDirectoryCommand = await _testDataHelper.PageDirectories().CreateAddCommandAsync(uniqueData);
            var parentDirectoryId = await contentRepository
                .PageDirectories()
                .AddAsync(parentDirectoryCommand);

            var directoryId = await _testDataHelper.PageDirectories().AddAsync("Dir-1", parentDirectoryId);

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
}
