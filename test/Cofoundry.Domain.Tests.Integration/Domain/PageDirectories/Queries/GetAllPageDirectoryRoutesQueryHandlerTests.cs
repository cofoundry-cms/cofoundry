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

        [Fact]
        public async Task MapsAccessRules()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(MapsAccessRules);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var rule1Command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea1.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(rule1Command);

            var rule2Command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.NotFound
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(rule2Command);

            var allDirectories = await contentRepository
                .PageDirectories()
                .GetAll()
                .AsRoutes()
                .ExecuteAsync();

            var directory = allDirectories.SingleOrDefault(d => d.PageDirectoryId == directoryId);

            using (new AssertionScope())
            {
                directory.Should().NotBeNull();
                directory.AccessRules.Should().NotBeNull().And.HaveCount(2);

                var rule1 = directory.AccessRules.SingleOrDefault(r => r.UserAreaCode == rule1Command.UserAreaCode);
                rule1.Should().NotBeNull();
                rule1.RoleId.Should().Be(rule1Command.RoleId);
                rule1.ViolationAction.Should().Be(rule1Command.ViolationAction);

                var rule2 = directory.AccessRules.SingleOrDefault(r => r.UserAreaCode == rule2Command.UserAreaCode);
                rule2.Should().NotBeNull();
                rule2.RoleId.Should().Be(rule2Command.RoleId);
                rule2.ViolationAction.Should().Be(rule2Command.ViolationAction);
            }
        }
    }
}
