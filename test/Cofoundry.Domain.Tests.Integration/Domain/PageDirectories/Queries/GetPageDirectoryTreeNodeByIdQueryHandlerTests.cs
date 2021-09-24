using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageDirectoryTreeNodeByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageDirTreeNodeByIdCHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageDirectoryTreeNodeByIdQueryHandlerTests(
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
            await _testDataHelper.PageDirectories().AddAsync("Dir-1-A", directoryId);
            await _testDataHelper.PageDirectories().AddAsync("Dir-1-B", directoryId);
            await _testDataHelper.PageDirectories().AddAsync("Dir-1-C", directoryId);

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
}
