﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class GetPageAccessRuleSetDetailsByIdPageQueryHandlerTests
{
    const string UNIQUE_PREFIX = "GPagAccDetailsByIdQHT ";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public GetPageAccessRuleSetDetailsByIdPageQueryHandlerTests(
        IntegrationTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task ReturnsRequestedPage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(ReturnsRequestedPage);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var accessDetails = await contentRepository
            .Pages()
            .AccessRules()
            .GetByPageId(pageId)
            .AsDetails()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            accessDetails.Should().NotBeNull();
            accessDetails?.PageId.Should().Be(pageId);
            accessDetails?.AccessRules.Should().NotBeNull().And.BeEmpty();
            accessDetails?.InheritedAccessRules.Should().NotBeNull().And.BeEmpty();
            accessDetails?.UserAreaForSignInRedirect.Should().BeNull();
            accessDetails?.ViolationAction.Should().Be(AccessRuleViolationAction.Error);
        }
    }

    [Fact]
    public async Task DoesNotReturnDeletedPage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(DoesNotReturnDeletedPage);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Pages()
            .DeleteAsync(pageId);

        var pageAccess = await contentRepository
            .Pages()
            .AccessRules()
            .GetByPageId(pageId)
            .AsDetails()
            .ExecuteAsync();

        pageAccess.Should().BeNull();
    }

    [Fact]
    public async Task MapsBasicData()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(MapsBasicData);

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
            ViolationAction = AccessRuleViolationAction.NotFound,
            AccessRules = [
                new()
                {
                    UserAreaCode = app.SeededEntities.TestUserArea2.UserAreaCode,
                    RoleId = app.SeededEntities.TestUserArea2.RoleA.RoleId
                },
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

        var accessDetails = await contentRepository
            .Pages()
            .AccessRules()
            .GetByPageId(pageId)
            .AsDetails()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            accessDetails.Should().NotBeNull();

            if (accessDetails == null)
            {
                return;
            }

            accessDetails.UserAreaForSignInRedirect.Should().NotBeNull();
            accessDetails.UserAreaForSignInRedirect?.UserAreaCode.Should().Be(userArea1.UserAreaCode);
            accessDetails.UserAreaForSignInRedirect?.Name.Should().Be(userArea1.Definition.Name);
            accessDetails.ViolationAction.Should().Be(command.ViolationAction);
            accessDetails.InheritedAccessRules.Should().NotBeNull().And.BeEmpty();
            accessDetails.AccessRules.Should().HaveCount(3);

            var rule1 = accessDetails.AccessRules.FirstOrDefault();
            ValidateRuleMapping(rule1, userArea1, pageId, true);

            var rule2 = accessDetails.AccessRules.Skip(1).FirstOrDefault();
            ValidateRuleMapping(rule2, userArea2, pageId, false);

            var rule3 = accessDetails.AccessRules.Skip(2).FirstOrDefault();
            ValidateRuleMapping(rule3, userArea2, pageId, true);
        }

        static void ValidateRuleMapping(
            PageAccessRuleSummary? rule,
            SeedData.TestUserAreaInfo userArea,
            int pageId,
            bool hasRole
            )
        {
            rule.Should().NotBeNull();

            if (rule == null)
            {
                return;
            }

            rule.PageAccessRuleId.Should().BePositive();
            rule.PageId.Should().Be(pageId);

            rule.UserArea.Should().NotBeNull();
            rule.UserArea.UserAreaCode.Should().Be(userArea.UserAreaCode);
            rule.UserArea.Name.Should().Be(userArea.Definition.Name);

            if (hasRole)
            {
                rule.Role.Should().NotBeNull();
                rule.Role?.RoleId.Should().Be(userArea.RoleA.RoleId);
                rule.Role?.Title.Should().NotBeNullOrWhiteSpace();
            }
            else
            {
                rule.Role.Should().BeNull();
            }
        }
    }

    [Fact]
    public async Task MapsInheritedRules()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(MapsInheritedRules);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var userArea1 = app.SeededEntities.TestUserArea1;
        var userArea2 = app.SeededEntities.TestUserArea2;

        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.PageDirectories().AddAccessRuleAsync(directory1Id, userArea1.UserAreaCode, userArea1.RoleA.RoleId, c =>
        {
            c.AccessRules = c.AccessRules.Append(new()
            {
                UserAreaCode = userArea2.UserAreaCode
            }).ToArray();
            c.UserAreaCodeForSignInRedirect = userArea2.UserAreaCode;
            c.ViolationAction = AccessRuleViolationAction.NotFound;
        });
        var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory1Id);
        var directory3Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory2Id);
        await app.TestData.PageDirectories().AddAccessRuleAsync(directory3Id, userArea2.UserAreaCode, userArea2.RoleA.RoleId);
        var directory4Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory3Id);

        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directory4Id);
        var accessDetails = await contentRepository
            .Pages()
            .AccessRules()
            .GetByPageId(pageId)
            .AsDetails()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            accessDetails.Should().NotBeNull();

            if (accessDetails == null)
            {
                return;
            }

            accessDetails.AccessRules.Should().NotBeNull().And.BeEmpty();
            accessDetails.InheritedAccessRules.Should().HaveCount(2);

            var ruleSet1 = accessDetails.InheritedAccessRules.FirstOrDefault();
            if (ruleSet1 == null)
            {
                return;
            }

            ruleSet1.UserAreaForSignInRedirect.Should().NotBeNull();
            ruleSet1.UserAreaForSignInRedirect?.UserAreaCode.Should().Be(userArea2.UserAreaCode);
            ruleSet1.UserAreaForSignInRedirect?.Name.Should().Be(userArea2.Definition.Name);
            ruleSet1.ViolationAction.Should().Be(AccessRuleViolationAction.NotFound);
            ruleSet1.AccessRules.Should().HaveCount(2);

            var rule1 = ruleSet1.AccessRules.FirstOrDefault();
            ValidateRuleMapping(rule1, userArea1, directory1Id, true);

            var rule2 = ruleSet1.AccessRules.Skip(1).FirstOrDefault();
            ValidateRuleMapping(rule2, userArea2, directory1Id, false);

            var ruleSet2 = accessDetails.InheritedAccessRules.Skip(1).FirstOrDefault();
            if (ruleSet2 == null)
            {
                return;
            }
            ruleSet2.UserAreaForSignInRedirect.Should().BeNull();
            ruleSet2.ViolationAction.Should().Be(AccessRuleViolationAction.Error);
            ruleSet2.AccessRules.Should().HaveCount(1);

            var rule3 = ruleSet2.AccessRules.FirstOrDefault();
            ValidateRuleMapping(rule3, userArea2, directory3Id, true);
        }

        static void ValidateRuleMapping(
            PageDirectoryAccessRuleSummary? rule,
            SeedData.TestUserAreaInfo userArea,
            int directoryId,
            bool hasRole
            )
        {
            rule.Should().NotBeNull();

            if (rule == null)
            {
                return;
            }

            rule.PageDirectoryAccessRuleId.Should().BePositive();
            rule.PageDirectoryId.Should().Be(directoryId);

            rule.UserArea.Should().NotBeNull();
            rule.UserArea.UserAreaCode.Should().Be(userArea.UserAreaCode);
            rule.UserArea.Name.Should().Be(userArea.Definition.Name);

            if (hasRole)
            {
                rule.Role.Should().NotBeNull();
                rule.Role?.RoleId.Should().Be(userArea.RoleA.RoleId);
                rule.Role?.Title.Should().NotBeNullOrWhiteSpace();
            }
            else
            {
                rule.Role.Should().BeNull();
            }
        }
    }

}
