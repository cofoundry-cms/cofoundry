using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Commands;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class UpdatePageDirectoryAccessRuleSetCommandHandlerTests
{
    const string UNIQUE_PREFIX = "UpdPageDirAccessRulesCHT ";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public UpdatePageDirectoryAccessRuleSetCommandHandlerTests(
        IntegrationTestApplicationFactory appFactory
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

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var accessRules = await dbContext
            .PageDirectoryAccessRules
            .AsNoTracking()
            .FilterByPageDirectoryId(directoryId)
            .ToListAsync();

        using (new AssertionScope())
        {
            accessRules.Should().HaveCount(1);
            var accessRule = accessRules.Single();
            accessRule.CreateDate.Should().NotBeDefault();
            accessRule.CreatorId.Should().BePositive();
            accessRule.PageDirectoryAccessRuleId.Should().BePositive();
            accessRule.PageDirectoryId.Should().Be(directoryId);
            accessRule.RoleId.Should().BeNull();
            accessRule.UserAreaCode.Should().Be(app.SeededEntities.TestUserArea1.UserAreaCode);
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

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea2.RoleA.RoleId
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var accessRules = await dbContext
            .PageDirectoryAccessRules
            .AsNoTracking()
            .FilterByPageDirectoryId(directoryId)
            .ToListAsync();

        using (new AssertionScope())
        {
            accessRules.Should().HaveCount(1);
            var accessRule = accessRules.Single();
            accessRule.RoleId.Should().Be(app.SeededEntities.TestUserArea2.RoleA.RoleId);
            accessRule.UserAreaCode.Should().Be(app.SeededEntities.TestUserArea2.UserAreaCode);
        }
    }

    [Theory]
    [InlineData(AccessRuleViolationAction.Error)]
    [InlineData(AccessRuleViolationAction.NotFound)]
    public async Task CanAddViolationAction(AccessRuleViolationAction action)
    {
        var uniqueData = $"{UNIQUE_PREFIX}{nameof(CanAddViolationAction)}{action.ToString().AsSpan(0, 2)}";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            ViolationAction = action,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea2.RoleA.RoleId
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var directory = await dbContext
            .PageDirectories
            .AsNoTracking()
            .FilterById(directoryId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            directory.Should().NotBeNull();
            directory?.AccessRuleViolationActionId.Should().Be((int)action);
        }
    }

    [Fact]
    public async Task WhenRoleNotInUserArea_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenRoleNotInUserArea_Throws);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleA.RoleId
                }]
        };

        await contentRepository
            .Awaiting(r => r.PageDirectories().AccessRules().UpdateAsync(command))
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMemberNames("RoleId");
    }

    [Fact]
    public async Task CanUpdateWithSameData()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanUpdateWithSameData);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var userArea = app.SeededEntities.TestUserArea1;

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            UserAreaCodeForSignInRedirect = userArea.UserAreaCode,
            ViolationAction = AccessRuleViolationAction.NotFound,
            AccessRules = [
                new()
                {
                    UserAreaCode = userArea.UserAreaCode
                },
                new()
                {
                    UserAreaCode = userArea.UserAreaCode,
                    RoleId = userArea.RoleA.RoleId
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var directory = await dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(directoryId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            directory.Should().NotBeNull();

            if (directory != null)
            {
                directory.AccessRuleViolationActionId.Should().Be((int)command.ViolationAction);
                directory.UserAreaCodeForSignInRedirect.Should().Be(command.UserAreaCodeForSignInRedirect);

                var userAreaAccessRule = directory.AccessRules.SingleOrDefault(a => !a.RoleId.HasValue);
                userAreaAccessRule.Should().NotBeNull();
                var roleAccessArea = directory.AccessRules.SingleOrDefault(a => a.RoleId.HasValue);
                roleAccessArea.Should().NotBeNull();
                roleAccessArea?.UserAreaCode.Should().Be(userArea.UserAreaCode);
            }
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

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            UserAreaCodeForSignInRedirect = app.SeededEntities.TestUserArea1.UserAreaCode,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>(directoryId));
        EntityNotFoundException.ThrowIfNull(updateCommand, directoryId);

        updateCommand.UserAreaCodeForSignInRedirect = app.SeededEntities.TestUserArea2.UserAreaCode;
        var userAreaAccessRuleCommand = updateCommand.AccessRules.Single();
        userAreaAccessRuleCommand.UserAreaCode = updateCommand.UserAreaCodeForSignInRedirect;

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var directory = await dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(directoryId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            directory.Should().NotBeNull();
            if (directory != null)
            {
                directory.UserAreaCodeForSignInRedirect.Should().Be(updateCommand.UserAreaCodeForSignInRedirect);

                var userAreaAccessRule = directory.AccessRules.SingleOrDefault();
                userAreaAccessRule.Should().NotBeNull();
                userAreaAccessRule?.UserAreaCode.Should().Be(userAreaAccessRuleCommand.UserAreaCode);
                userAreaAccessRule?.RoleId.Should().BeNull();
            }
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
        var userArea1 = app.SeededEntities.TestUserArea1;
        var userArea2 = app.SeededEntities.TestUserArea2;

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            UserAreaCodeForSignInRedirect = userArea1.UserAreaCode,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleA.RoleId
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>(directoryId));
        EntityNotFoundException.ThrowIfNull(updateCommand, directoryId);

        updateCommand.UserAreaCodeForSignInRedirect = userArea2.UserAreaCode;
        var roleAccessRuleCommand = updateCommand.AccessRules.Single();
        roleAccessRuleCommand.UserAreaCode = userArea2.UserAreaCode;
        roleAccessRuleCommand.RoleId = userArea2.RoleA.RoleId;

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var directory = await dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(directoryId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {

            directory.Should().NotBeNull();
            if (directory != null)
            {
                directory.UserAreaCodeForSignInRedirect.Should().Be(updateCommand.UserAreaCodeForSignInRedirect);

                var userAreaAccessRule = directory.AccessRules.SingleOrDefault();
                userAreaAccessRule.Should().NotBeNull();
                userAreaAccessRule?.UserAreaCode.Should().Be(roleAccessRuleCommand.UserAreaCode);
                userAreaAccessRule?.RoleId.Should().Be(roleAccessRuleCommand.RoleId);
            }
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

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleA.RoleId
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>(directoryId));
        EntityNotFoundException.ThrowIfNull(updateCommand, directoryId);
        var userAreaAccessRuleCommand = updateCommand.AccessRules.Single();
        userAreaAccessRuleCommand.RoleId = null;

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var directory = await dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(directoryId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            directory.Should().NotBeNull();
            if (directory != null)
            {
                directory.UserAreaCodeForSignInRedirect.Should().BeNull();

                var userAreaAccessRule = directory.AccessRules.SingleOrDefault();
                userAreaAccessRule.Should().NotBeNull();
                userAreaAccessRule?.UserAreaCode.Should().Be(userAreaAccessRuleCommand.UserAreaCode);
                userAreaAccessRule?.RoleId.Should().BeNull();
            }
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

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            ViolationAction = AccessRuleViolationAction.Error,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>(directoryId));
        EntityNotFoundException.ThrowIfNull(updateCommand, directoryId);
        updateCommand.ViolationAction = AccessRuleViolationAction.NotFound;

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var directory = await dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(directoryId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            directory.Should().NotBeNull();
            directory?.UserAreaCodeForSignInRedirect.Should().BeNull();
            directory?.AccessRuleViolationActionId.Should().Be((int)updateCommand.ViolationAction);

            directory?.AccessRules.Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task CanDelete()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanDelete);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleA.RoleId
                },
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>(directoryId));
        EntityNotFoundException.ThrowIfNull(updateCommand, directoryId);
        var accessRuleCommand = updateCommand.AccessRules.Single(r => r.RoleId == app.SeededEntities.TestUserArea1.RoleA.RoleId);
        updateCommand.AccessRules = updateCommand
            .AccessRules
            .Except([accessRuleCommand])
            .ToArray();

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var accessRules = await dbContext
            .PageDirectoryAccessRules
            .AsNoTracking()
            .FilterByPageDirectoryId(directoryId)
            .ToListAsync();

        using (new AssertionScope())
        {
            accessRules.Should().HaveCount(1);
            var accessRule = accessRules.SingleOrDefault();
            accessRule?.UserAreaCode.Should().Be(app.SeededEntities.TestUserArea2.UserAreaCode);
            accessRule?.RoleId.Should().BeNull();
        }
    }

    [Fact]
    public async Task CanCombineMultipleOperations()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanCombineMultipleOperations);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId,
            UserAreaCodeForSignInRedirect = app.SeededEntities.TestUserArea1.UserAreaCode,
            ViolationAction = AccessRuleViolationAction.NotFound,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleA.RoleId
                },
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode
                }]
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>(directoryId));
        EntityNotFoundException.ThrowIfNull(updateCommand, directoryId);
        updateCommand.UserAreaCodeForSignInRedirect = null;
        updateCommand.ViolationAction = AccessRuleViolationAction.Error;

        var ruleToRemove = updateCommand.AccessRules.Single(r => r.UserAreaCode == app.SeededEntities.TestUserArea2.UserAreaCode);
        var accessRules = updateCommand
            .AccessRules
            .Except([ruleToRemove])
            .ToList();
        var ruleToUpdate = updateCommand.AccessRules.Single(r => r.RoleId == app.SeededEntities.TestUserArea1.RoleA.RoleId);
        ruleToUpdate.RoleId = null;
        accessRules.Add(new()
        {
            UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
            RoleId = app.SeededEntities.TestUserArea2.RoleA.RoleId
        });

        updateCommand.AccessRules = accessRules;

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var directory = await dbContext
            .PageDirectories
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(directoryId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            directory.Should().NotBeNull();

            if (directory != null)
            {
                directory.UserAreaCodeForSignInRedirect.Should().BeNull();
                directory.AccessRuleViolationActionId.Should().Be((int)updateCommand.ViolationAction);
                directory.AccessRules.Should().HaveCount(2);

                var userArea1AccessRule = directory.AccessRules.SingleOrDefault(r => r.UserAreaCode == app.SeededEntities.TestUserArea1.UserAreaCode);
                userArea1AccessRule.Should().NotBeNull();
                userArea1AccessRule?.RoleId.Should().BeNull();

                var userArea2AccessRule = directory.AccessRules.SingleOrDefault(r => r.UserAreaCode == app.SeededEntities.TestUserArea2.UserAreaCode);
                userArea2AccessRule.Should().NotBeNull();
                userArea2AccessRule?.RoleId.Should().Be(app.SeededEntities.TestUserArea2.RoleA.RoleId);
            }
        }
    }

    [Fact]
    public async Task WhenUpdated_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenUpdated_SendsMessage);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var command = new UpdatePageDirectoryAccessRuleSetCommand()
        {
            PageDirectoryId = directoryId
        };

        await contentRepository
            .PageDirectories()
            .AccessRules()
            .UpdateAsync(command);

        app.Mocks
            .CountMessagesPublished<PageDirectoryAccessRulesUpdatedMessage>(m => m.PageDirectoryId == directoryId)
            .Should().Be(1);
    }
}
