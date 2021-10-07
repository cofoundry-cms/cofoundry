using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class DeletePageDirectoryAccessRuleCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DelPageDirAccessRuleCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public DeletePageDirectoryAccessRuleCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanDelete()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanDelete);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var addCommand = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea1.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            var accessRuleId = await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .DeleteAsync(accessRuleId);

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(accessRuleId)
                .SingleOrDefaultAsync();

            accessRule.Should().BeNull();
        }

        [Fact]
        public async Task WhenDeleted_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenDeleted_SendsMessage);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var addCommand = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea1.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            var accessRuleId = await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .DeleteAsync(accessRuleId);

            app.Mocks
                .CountMessagesPublished<PageDirectoryAccessRuleDeletedMessage>(m => m.PageDirectoryId == directoryId && m.PageDirectoryAccessRuleId == accessRuleId)
                .Should().Be(1);
        }
    }
}
