using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class DeletePageAccessRuleCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DelPageAccessRuleCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public DeletePageAccessRuleCommandHandlerTests(
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
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var addCommand = new AddPageAccessRuleCommand()
            {
                PageId = pageId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea1.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            var accessRuleId = await contentRepository
                .Pages()
                .AccessRules()
                .AddAsync(addCommand);

            await contentRepository
                .Pages()
                .AccessRules()
                .DeleteAsync(accessRuleId);

            var accessRule = await dbContext
                .PageAccessRules
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
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var addCommand = new AddPageAccessRuleCommand()
            {
                PageId = pageId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea1.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            var accessRuleId = await contentRepository
                .Pages()
                .AccessRules()
                .AddAsync(addCommand);

            await contentRepository
                .Pages()
                .AccessRules()
                .DeleteAsync(accessRuleId);

            app.Mocks
                .CountMessagesPublished<PageAccessRuleDeletedMessage>(m => m.PageId == pageId && m.PageAccessRuleId == accessRuleId)
                .Should().Be(1);
        }
    }
}
