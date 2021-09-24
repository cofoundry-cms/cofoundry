using Cofoundry.Core;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageDirectoryTreeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GAllPageDirTreeCHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageDirectoryTreeQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task RootNodeIsRootDirectory()
        {
            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var treeRoot = await contentRepository
                .PageDirectories()
                .GetAll()
                .AsTree()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                treeRoot.Should().NotBeNull();
                treeRoot.AuditData.Should().NotBeNull();
                treeRoot.AuditData.Creator.Should().NotBeNull();
                treeRoot.AuditData.CreateDate.Should().NotBeDefault();
                treeRoot.ChildPageDirectories.Should().NotBeNull();
                treeRoot.Depth.Should().Be(0);
                treeRoot.UrlPath.Should().BeEmpty();
                treeRoot.FullUrlPath.Should().Be("/");
                treeRoot.ParentPageDirectoryId.Should().BeNull();
                treeRoot.ParentPageDirectory.Should().BeNull();
            }
        }

        [Fact]
        public async Task ReturnsTreeNodes()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsTreeNodes);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var parentDirectoryCommand = await _testDataHelper.PageDirectories().CreateAddCommandAsync(uniqueData);
            var parentDirectoryId = await contentRepository
                .PageDirectories()
                .AddAsync(parentDirectoryCommand);

            var directory1Id = await _testDataHelper.PageDirectories().AddAsync("dir-1", parentDirectoryId);
            var directory2Id = await _testDataHelper.PageDirectories().AddAsync("dir-2", parentDirectoryId);
            var directory2AId = await _testDataHelper.PageDirectories().AddAsync("dir-2-A", directory2Id);

            var tree = await contentRepository
                .PageDirectories()
                .GetAll()
                .AsTree()
                .ExecuteAsync();

            var parentDirectory = tree.ChildPageDirectories.SingleOrDefault(d => d.PageDirectoryId == parentDirectoryId);
            var directory1 = parentDirectory.ChildPageDirectories.SingleOrDefault(d => d.PageDirectoryId == directory1Id);
            var directory2 = parentDirectory.ChildPageDirectories.SingleOrDefault(d => d.PageDirectoryId == directory2Id);
            var directory2A = directory2.ChildPageDirectories.SingleOrDefault(d => d.PageDirectoryId == directory2AId);

            var parentFullPath = "/" + parentDirectoryCommand.UrlPath;

            using (new AssertionScope())
            {
                parentDirectory.Should().NotBeNull();
                parentDirectory.AuditData.Should().NotBeNull();
                parentDirectory.AuditData.Creator.Should().NotBeNull();
                parentDirectory.AuditData.CreateDate.Should().NotBeDefault();
                parentDirectory.ChildPageDirectories.Should().NotBeNull().And.HaveCount(2);
                parentDirectory.Depth.Should().Be(1);
                parentDirectory.Name.Should().Be(parentDirectoryCommand.Name);
                parentDirectory.UrlPath.Should().Be(parentDirectoryCommand.UrlPath);
                parentDirectory.FullUrlPath.Should().Be(parentFullPath);
                parentDirectory.ParentPageDirectoryId.Should().Be(tree.PageDirectoryId);
                parentDirectory.ParentPageDirectory.Should().Be(tree);

                directory1.Should().NotBeNull();
                directory1.Depth.Should().Be(2);
                directory1.ChildPageDirectories.Should().NotBeNull().And.BeEmpty();
                directory1.ParentPageDirectory.Should().Be(parentDirectory);
                directory1.ParentPageDirectoryId.Should().Be(parentDirectoryId);
                directory1.FullUrlPath.Should().Be(parentFullPath + "/dir-1");

                directory2.Should().NotBeNull();
                directory2.Depth.Should().Be(2);
                directory2.ChildPageDirectories.Should().NotBeNull().And.HaveCount(1);
                directory2.ParentPageDirectory.Should().Be(parentDirectory);
                directory2.ParentPageDirectoryId.Should().Be(parentDirectoryId);
                directory2.FullUrlPath.Should().Be(parentFullPath + "/dir-2");

                directory2A.Should().NotBeNull();
                directory2A.Depth.Should().Be(3);
                directory2A.ChildPageDirectories.Should().NotBeNull().And.BeEmpty();
                directory2A.ParentPageDirectory.Should().Be(directory2);
                directory2A.ParentPageDirectoryId.Should().Be(directory2Id);
                directory2A.FullUrlPath.Should().Be(parentFullPath + "/dir-2/dir-2-a");
            }
        }
    }
}
