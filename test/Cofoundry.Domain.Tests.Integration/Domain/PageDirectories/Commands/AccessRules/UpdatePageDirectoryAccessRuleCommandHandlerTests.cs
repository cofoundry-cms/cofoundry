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
    public class UpdatePageDirectoryAccessRuleCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "UpdPageDirAccessRuleCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public UpdatePageDirectoryAccessRuleCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanUpdateWithSameData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanUpdateWithSameData);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var addCommand = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea1.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            var updateCommand = MapToUpdateCommand(addCommand);

            await contentRepository
                .Awaiting(r => r.PageDirectories().AccessRules().UpdateAsync(updateCommand))
                .Should()
                .NotThrowAsync();

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(addCommand.OutputPageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                accessRule.Should().NotBeNull();
                accessRule.PageDirectoryAccessRuleId.Should().Be(addCommand.OutputPageDirectoryAccessRuleId);
                accessRule.PageDirectoryId.Should().Be(addCommand.PageDirectoryId);
                accessRule.RoleId.Should().Be(addCommand.RoleId);
                accessRule.RouteAccessRuleViolationActionId.Should().Be((int)addCommand.ViolationAction);
                accessRule.UserAreaCode.Should().Be(addCommand.UserAreaCode);
            }
        }

        [Fact]
        public async Task CanChangeUserArea()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanChangeUserArea);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var addCommand = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            var updateCommand = MapToUpdateCommand(addCommand);
            updateCommand.UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode;

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .UpdateAsync(updateCommand);

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(addCommand.OutputPageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                accessRule.Should().NotBeNull();
                accessRule.UserAreaCode.Should().Be(updateCommand.UserAreaCode);
                accessRule.RoleId.Should().BeNull();
            }
        }

        [Fact]
        public async Task CanChangeRole()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanChangeRole);

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

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            var updateCommand = MapToUpdateCommand(addCommand);
            updateCommand.UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode;
            updateCommand.RoleId = app.SeededEntities.TestUserArea2.RoleId;

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .UpdateAsync(updateCommand);

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(addCommand.OutputPageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                accessRule.Should().NotBeNull();
                accessRule.UserAreaCode.Should().Be(updateCommand.UserAreaCode);
                accessRule.RoleId.Should().Be(updateCommand.RoleId);
            }
        }

        [Fact]
        public async Task CanRemoveRole()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanRemoveRole);

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

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            var updateCommand = MapToUpdateCommand(addCommand);
            updateCommand.RoleId = null;

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .UpdateAsync(updateCommand);

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(addCommand.OutputPageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                accessRule.Should().NotBeNull();
                accessRule.UserAreaCode.Should().Be(updateCommand.UserAreaCode);
                accessRule.RoleId.Should().BeNull();
            }
        }

        [Fact]
        public async Task CanChangeViolationAction()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanChangeViolationAction);

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

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            var updateCommand = MapToUpdateCommand(addCommand);
            updateCommand.ViolationAction = RouteAccessRuleViolationAction.NotFound;

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .UpdateAsync(updateCommand);

            var accessRule = await dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .FilterById(addCommand.OutputPageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                accessRule.Should().NotBeNull();
                accessRule.RouteAccessRuleViolationActionId.Should().Be((int)updateCommand.ViolationAction);
            }
        }

        [Fact]
        public async Task WhenCofoundryAdminUserArea_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenCofoundryAdminUserArea_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var addCommand = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            var updateCommand = MapToUpdateCommand(addCommand);
            updateCommand.UserAreaCode = CofoundryAdminUserArea.AreaCode;

            await contentRepository
                .Awaiting(r => r.PageDirectories().AccessRules().UpdateAsync(updateCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(updateCommand.UserAreaCode));
        }

        [Fact]
        public async Task WhenRoleNotInUserArea_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenRoleNotInUserArea_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var addCommand = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            var updateCommand = MapToUpdateCommand(addCommand);
            updateCommand.RoleId = app.SeededEntities.TestUserArea2.RoleId;

            await contentRepository
                .Awaiting(r => r.PageDirectories().AccessRules().UpdateAsync(updateCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(updateCommand.RoleId));
        }

        [Fact]
        public async Task WhenNotUnique_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenNotUnique_Throws);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var area1AccessRuleId = await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(new AddPageDirectoryAccessRuleCommand()
                {
                    PageDirectoryId = directoryId,
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleId,
                    ViolationAction = RouteAccessRuleViolationAction.Error
                });

            var addCommand = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea2.RoleId,
                ViolationAction = RouteAccessRuleViolationAction.Error
            };

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            var updateCommand = MapToUpdateCommand(addCommand);
            updateCommand.PageDirectoryAccessRuleId = area1AccessRuleId;

            await contentRepository
                .Awaiting(r => r.PageDirectories().AccessRules().UpdateAsync(updateCommand))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException<PageDirectoryAccessRule>>()
                .WithMemberNames(nameof(updateCommand.RoleId));
        }

        [Fact]
        public async Task WhenUpdated_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUpdated_SendsMessage);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

            var addCommand = new AddPageDirectoryAccessRuleCommand()
            {
                PageDirectoryId = directoryId,
                UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                ViolationAction = RouteAccessRuleViolationAction.RedirectToLogin
            };

            var accessRuleId = await contentRepository
                .PageDirectories()
                .AccessRules()
                .AddAsync(addCommand);

            await contentRepository
                .PageDirectories()
                .AccessRules()
                .UpdateAsync(MapToUpdateCommand(addCommand));

            app.Mocks
                .CountMessagesPublished<PageDirectoryAccessRuleUpdatedMessage>(m => m.PageDirectoryId == directoryId && m.PageDirectoryAccessRuleId == accessRuleId)
                .Should().Be(1);
        }

        private UpdatePageDirectoryAccessRuleCommand MapToUpdateCommand(AddPageDirectoryAccessRuleCommand command)
        {
            return new UpdatePageDirectoryAccessRuleCommand()
            {
                PageDirectoryAccessRuleId = command.OutputPageDirectoryAccessRuleId,
                RoleId = command.RoleId,
                UserAreaCode = command.UserAreaCode,
                ViolationAction = command.ViolationAction
            };
        }
    }
}
