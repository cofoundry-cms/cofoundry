using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class AddPageDirectoryAccessRuleCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "AddPageDirAccessRuleCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public AddPageDirectoryAccessRuleCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task WithUserArea_CanAdd()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithUserArea_CanAdd);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(command);

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(command.OutputPageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                command.OutputPageDirectoryAccessRuleId.Should().BePositive();
                accessRule.Should().NotBeNull();
                accessRule.CreateDate.Should().NotBeDefault();
                accessRule.CreatorId.Should().BePositive();
                accessRule.PageDirectoryAccessRuleId.Should().Be(command.OutputPageDirectoryAccessRuleId);
                accessRule.PageDirectoryId.Should().Be(directoryId);
                accessRule.RoleId.Should().BeNull();
                accessRule.RouteAccessRuleViolationActionId.Should().Be((int)RouteAccessRuleViolationAction.Error);
                accessRule.UserAreaCode.Should().Be(command.UserAreaCode);
            }
        }

        [Fact]
        public async Task WithRoleId_CanAdd()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithRoleId_CanAdd);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea1.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.NotFound
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(command);

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(command.OutputPageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                accessRule.Should().NotBeNull();
                accessRule.RoleId.Should().Be(command.RoleId);
                accessRule.RouteAccessRuleViolationActionId.Should().Be((int)RouteAccessRuleViolationAction.NotFound);
                accessRule.UserAreaCode.Should().Be(command.UserAreaCode);
            }
        }

        [Theory]
        [InlineData(RouteAccessRuleViolationAction.Error)]
        [InlineData(RouteAccessRuleViolationAction.NotFound)]
        [InlineData(RouteAccessRuleViolationAction.RedirectToLogin)]
        public async Task CanAddViolationAction(RouteAccessRuleViolationAction action)
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanAddViolationAction) + action.ToString().Substring(0, 2);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var pageDirectoryAccessRuleId = await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(new AddPageDirectoryAccessRuleCommand()
                {
                    PageDirectoryId = directoryId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleId,
                    ViolationAction = action
                });

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(pageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                accessRule.Should().NotBeNull();
                accessRule.RouteAccessRuleViolationActionId.Should().Be((int)action);
            }
        }

        [Fact]
        public async Task WhenCofoundryAdminUserArea_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenCofoundryAdminUserArea_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = CofoundryAdminUserArea.AreaCode,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            };

            await contentRepository
                .Awaiting(r => r.PageDirectories().AccessRules().AddAsync(command))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(command.UserAreaCode));
        }

        [Fact]
        public async Task WhenRoleNotInUserArea_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenRoleNotInUserArea_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea1.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            };

            await contentRepository
                .Awaiting(r => r.PageDirectories().AccessRules().AddAsync(command))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(command.RoleId));
        }

        [Fact]
        public async Task CanAddMultiple()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanAddMultiple);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            await contentRepository.PageDirectories().AccessRules().AddAsync(new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            });

            await contentRepository.PageDirectories().AccessRules().AddAsync(new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            });

            await contentRepository.PageDirectories().AccessRules().AddAsync(new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea2.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            });

            var accessRules = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterByPageId(directoryId)
                .ToListAsync();

            accessRules.Should().HaveCount(3);
        }

        [Fact]
        public async Task WhenUserAreaNotUnique_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUserAreaNotUnique_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            };

            await contentRepository.PageDirectories().AccessRules().AddAsync(command);

            command.OutputPageDirectoryAccessRuleId = 0;

            await contentRepository
                .Awaiting(r => r.PageDirectories().AccessRules().AddAsync(command))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException>()
                .WithMemberNames(nameof(command.UserAreaCode));
        }

        [Fact]
        public async Task WhenRoleNotUnique_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenRoleNotUnique_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea2.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            };

            await contentRepository.PageDirectories().AccessRules().AddAsync(command);

            command.OutputPageDirectoryAccessRuleId = 0;

            await contentRepository
                .Awaiting(r => r.PageDirectories().AccessRules().AddAsync(command))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException>()
                .WithMemberNames(nameof(command.RoleId));
        }

        [Fact]
        public async Task WhenAdded_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenAdded_SendsMessage);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var command = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            };

            var accessRuleId = await contentRepository.PageDirectories().AccessRules().AddAsync(command);

            app.Mocks
                .CountMessagesPublished<PageDirectoryAccessRuleAddedMessage>(m => m.PageDirectoryId == directoryId && m.PageDirectoryAccessRuleId == accessRuleId)
                .Should().Be(1);
        }
    }
}
