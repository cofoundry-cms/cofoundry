using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetAllPageDirectoryRoutesQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GAllPageDirRoutesQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetAllPageDirectoryRoutesQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task IncludesRootDirectory()
        {
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var allDirectories = await contentRepository
                .PageDirectories()
                .GetAll()
                .AsRoutes()
                .ExecuteAsync();

            var root = allDirectories.SingleOrDefault(d => !d.ParentPageDirectoryId.HasValue);

            using (new AssertionScope())
            {
                root.Should().NotBeNull();
                root.UrlPath.Should().BeEmpty();
                root.FullUrlPath.Should().Be("/");
                root.IsSiteRoot().Should().BeTrue();
                root.LocaleVariations.Should().NotBeNull().And.BeEmpty();
            }
        }

        [Fact]
        public async Task ReturnsAllMappedRoutes()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsAllMappedRoutes);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var parentDirectoryCommand = await app.TestData.PageDirectories().CreateAddCommandAsync(uniqueData);
            var parentDirectoryId = await contentRepository
                .PageDirectories()
                .AddAsync(parentDirectoryCommand);

            var directory1Id = await app.TestData.PageDirectories().AddAsync("dir-1", parentDirectoryId);
            var directory2Id = await app.TestData.PageDirectories().AddAsync("dir-2", parentDirectoryId);
            var directory2AId = await app.TestData.PageDirectories().AddAsync("dir-2-A", directory2Id);

            var allDirectories = await contentRepository
                .PageDirectories()
                .GetAll()
                .AsRoutes()
                .ExecuteAsync();

            var root = allDirectories.SingleOrDefault(d => !d.ParentPageDirectoryId.HasValue);
            var parentDirectory = allDirectories.SingleOrDefault(d => d.PageDirectoryId == parentDirectoryId);
            var directory1 = allDirectories.SingleOrDefault(d => d.PageDirectoryId == directory1Id);
            var directory2 = allDirectories.SingleOrDefault(d => d.PageDirectoryId == directory2Id);
            var directory2A = allDirectories.SingleOrDefault(d => d.PageDirectoryId == directory2AId);

            var parentFullPath = "/" + parentDirectoryCommand.UrlPath;

            using (new AssertionScope())
            {
                parentDirectory.Should().NotBeNull();
                parentDirectory.FullUrlPath.Should().Be(parentFullPath);
                parentDirectory.IsSiteRoot().Should().BeFalse();
                parentDirectory.LocaleVariations.Should().NotBeNull().And.BeEmpty();
                parentDirectory.Name.Should().Be(parentDirectoryCommand.Name);
                parentDirectory.ParentPageDirectoryId.Should().Be(root.PageDirectoryId);
                parentDirectory.UrlPath.Should().Be(parentDirectoryCommand.UrlPath);

                directory1.Should().NotBeNull();
                directory1.ParentPageDirectoryId.Should().Be(parentDirectoryId);
                directory1.FullUrlPath.Should().Be(parentFullPath + "/dir-1");

                directory2.Should().NotBeNull();
                directory2.ParentPageDirectoryId.Should().Be(parentDirectoryId);
                directory2.FullUrlPath.Should().Be(parentFullPath + "/dir-2");

                directory2A.Should().NotBeNull();
                directory2A.ParentPageDirectoryId.Should().Be(directory2Id);
                directory2A.FullUrlPath.Should().Be(parentFullPath + "/dir-2/dir-2-a");
            }
        }
    }
}
