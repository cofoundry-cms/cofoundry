﻿using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class UpdatePageAccessRuleSetCommandHandlerTests
{
    const string UNIQUE_PREFIX = "UpdPageAccessRulesCHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public UpdatePageAccessRuleSetCommandHandlerTests(
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                }]
        };

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var accessRules = await dbContext
            .PageAccessRules
            .AsNoTracking()
            .FilterByPageId(pageId)
            .ToListAsync();

        using (new AssertionScope())
        {
            accessRules.Should().HaveCount(1);
            var accessRule = accessRules.Single();
            accessRule.CreateDate.Should().NotBeDefault();
            accessRule.CreatorId.Should().BePositive();
            accessRule.PageAccessRuleId.Should().BePositive();
            accessRule.PageId.Should().Be(pageId);
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea2.RoleA.RoleId
                }]
        };

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var accessRules = await dbContext
            .PageAccessRules
            .AsNoTracking()
            .FilterByPageId(pageId)
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
            ViolationAction = action,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea2.RoleA.RoleId
                }]
        };

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var page = await dbContext
            .Pages
            .AsNoTracking()
            .FilterById(pageId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            page?.AccessRuleViolationActionId.Should().Be((int)action);
        }
    }

    [Fact]
    public async Task WhenRoleNotInUserArea_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenRoleNotInUserArea_Throws);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleA.RoleId
                }]
        };

        await contentRepository
            .Awaiting(r => r.Pages().AccessRules().UpdateAsync(command))
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);
        var userArea = app.SeededEntities.TestUserArea1;

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
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
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(pageId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();

            if (page == null)
            {
                return;
            }

            page.AccessRuleViolationActionId.Should().Be((int)command.ViolationAction);
            page.UserAreaCodeForSignInRedirect.Should().Be(command.UserAreaCodeForSignInRedirect);

            var userAreaAccessRule = page.AccessRules.Single(a => !a.RoleId.HasValue);
            userAreaAccessRule.Should().NotBeNull();
            var roleAccessArea = page.AccessRules.Single(a => a.RoleId.HasValue);
            roleAccessArea.Should().NotBeNull();
            roleAccessArea.UserAreaCode.Should().Be(userArea.UserAreaCode);
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
            UserAreaCodeForSignInRedirect = app.SeededEntities.TestUserArea1.UserAreaCode,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                }]
        };

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand>(pageId));
        EntityNotFoundException.ThrowIfNull(updateCommand, pageId);
        updateCommand.UserAreaCodeForSignInRedirect = app.SeededEntities.TestUserArea2.UserAreaCode;
        var userAreaAccessRuleCommand = updateCommand.AccessRules.Single();
        userAreaAccessRuleCommand.UserAreaCode = updateCommand.UserAreaCodeForSignInRedirect;

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(pageId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();

            if (page == null)
            {
                return;
            }

            page.UserAreaCodeForSignInRedirect.Should().Be(updateCommand.UserAreaCodeForSignInRedirect);

            var userAreaAccessRule = page.AccessRules.Single();
            userAreaAccessRule.Should().NotBeNull();
            userAreaAccessRule.UserAreaCode.Should().Be(userAreaAccessRuleCommand.UserAreaCode);
            userAreaAccessRule.RoleId.Should().BeNull();
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);
        var userArea1 = app.SeededEntities.TestUserArea1;
        var userArea2 = app.SeededEntities.TestUserArea2;

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
            UserAreaCodeForSignInRedirect = userArea1.UserAreaCode,
            AccessRules = [
                new()
                {
                    UserAreaCode = userArea1.UserAreaCode,
                    RoleId = userArea1.RoleA.RoleId
                }]
        };

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand>(pageId));
        EntityNotFoundException.ThrowIfNull(updateCommand, pageId);

        updateCommand.UserAreaCodeForSignInRedirect = userArea2.UserAreaCode;
        var roleAccessRuleCommand = updateCommand.AccessRules.Single();
        roleAccessRuleCommand.UserAreaCode = userArea2.UserAreaCode;
        roleAccessRuleCommand.RoleId = userArea2.RoleA.RoleId;

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(pageId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();

            if (page == null)
            {
                return;
            }

            page.UserAreaCodeForSignInRedirect.Should().Be(updateCommand.UserAreaCodeForSignInRedirect);

            var userAreaAccessRule = page.AccessRules.Single();
            userAreaAccessRule.Should().NotBeNull();
            userAreaAccessRule.UserAreaCode.Should().Be(roleAccessRuleCommand.UserAreaCode);
            userAreaAccessRule.RoleId.Should().Be(roleAccessRuleCommand.RoleId);
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea1.RoleA.RoleId
                }]
        };

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand>(pageId));
        EntityNotFoundException.ThrowIfNull(updateCommand, pageId);
        var userAreaAccessRuleCommand = updateCommand.AccessRules.Single();
        userAreaAccessRuleCommand.RoleId = null;

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(pageId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();

            if (page == null)
            {
                return;
            }

            page.UserAreaCodeForSignInRedirect.Should().BeNull();

            var userAreaAccessRule = page.AccessRules.Single();
            userAreaAccessRule.Should().NotBeNull();
            userAreaAccessRule.UserAreaCode.Should().Be(userAreaAccessRuleCommand.UserAreaCode);
            userAreaAccessRule.RoleId.Should().BeNull();
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
            ViolationAction = AccessRuleViolationAction.Error,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea1.UserAreaCode
                }]
        };

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand>(pageId));
        EntityNotFoundException.ThrowIfNull(updateCommand, pageId);
        updateCommand.ViolationAction = AccessRuleViolationAction.NotFound;

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(pageId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            page?.UserAreaCodeForSignInRedirect.Should().BeNull();
            page?.AccessRuleViolationActionId.Should().Be((int)updateCommand.ViolationAction);

            page?.AccessRules.Should().HaveCount(1);
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
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
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand>(pageId));
        EntityNotFoundException.ThrowIfNull(updateCommand, pageId);
        var accessRuleCommand = updateCommand.AccessRules.Single(r => r.RoleId == app.SeededEntities.TestUserArea1.RoleA.RoleId);
        updateCommand.AccessRules = updateCommand
            .AccessRules
            .Except([accessRuleCommand])
            .ToArray();

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var accessRules = await dbContext
            .PageAccessRules
            .AsNoTracking()
            .FilterByPageId(pageId)
            .ToArrayAsync();

        using (new AssertionScope())
        {
            accessRules.Should().HaveCount(1);
            var accessRule = accessRules.Single();
            accessRule.UserAreaCode.Should().Be(app.SeededEntities.TestUserArea2.UserAreaCode);
            accessRule.RoleId.Should().BeNull();
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
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId,
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
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        var updateCommand = await contentRepository.ExecuteQueryAsync(new GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand>(pageId));
        EntityNotFoundException.ThrowIfNull(updateCommand, pageId);
        updateCommand.UserAreaCodeForSignInRedirect = null;
        updateCommand.ViolationAction = AccessRuleViolationAction.Error;

        var ruleToUpdate = updateCommand.AccessRules.Single(r => r.RoleId == app.SeededEntities.TestUserArea1.RoleA.RoleId);
        ruleToUpdate.RoleId = null;
        var ruleToRemove = updateCommand.AccessRules.Single(r => r.UserAreaCode == app.SeededEntities.TestUserArea2.UserAreaCode);
        updateCommand.AccessRules = updateCommand.AccessRules
            .Except([ruleToRemove])
            .Append(new()
            {
                UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                RoleId = app.SeededEntities.TestUserArea2.RoleA.RoleId
            })
            .ToArray();

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(updateCommand);

        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.AccessRules)
            .FilterById(pageId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            if (page == null)
            {
                return;
            }

            page.UserAreaCodeForSignInRedirect.Should().BeNull();
            page.AccessRuleViolationActionId.Should().Be((int)updateCommand.ViolationAction);
            page.AccessRules.Should().HaveCount(2);

            var userArea1AccessRule = page.AccessRules.Single(r => r.UserAreaCode == app.SeededEntities.TestUserArea1.UserAreaCode);
            userArea1AccessRule.Should().NotBeNull();
            userArea1AccessRule.RoleId.Should().BeNull();

            var userArea2AccessRule = page.AccessRules.Single(r => r.UserAreaCode == app.SeededEntities.TestUserArea2.UserAreaCode);
            userArea2AccessRule.Should().NotBeNull();
            userArea2AccessRule.RoleId.Should().Be(app.SeededEntities.TestUserArea2.RoleA.RoleId);
        }
    }

    [Fact]
    public async Task WhenUpdated_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenUpdated_SendsMessage);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var command = new UpdatePageAccessRuleSetCommand()
        {
            PageId = pageId
        };

        await contentRepository
            .Pages()
            .AccessRules()
            .UpdateAsync(command);

        app.Mocks
            .CountMessagesPublished<PageAccessRulesUpdatedMessage>(m => m.PageId == pageId)
            .Should().Be(1);
    }
}
