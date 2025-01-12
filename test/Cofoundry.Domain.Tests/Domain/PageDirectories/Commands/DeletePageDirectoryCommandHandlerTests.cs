﻿using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain.Tests.PageDirectories.Commands;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class DeletePageDirectoryCommandHandlerTests
{
    const string UNIQUE_PREFIX = "DelPageDirectoryCHT ";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public DeletePageDirectoryCommandHandlerTests(
         IntegrationTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task WhenRootParent_DoesNotDeleteSiblings()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenRootParent_DoesNotDeleteSiblings);

        using var app = _appFactory.Create();
        var pageDirectory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");
        var pageDirectory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "2");

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        await contentRepository
            .PageDirectories()
            .DeleteAsync(pageDirectory1Id);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var directory1Exists = await DoesDirectoryExistsAsync(pageDirectory1Id);
        var directory2Exists = await DoesDirectoryExistsAsync(pageDirectory2Id);

        using (new AssertionScope())
        {
            directory1Exists.Should().BeFalse();
            directory2Exists.Should().BeTrue();
        }
    }

    [Fact]
    public async Task WhenHasChildren_DeletesChildren()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenHasChildren_DeletesChildren);

        using var app = _appFactory.Create();
        var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData + " P");
        var childDirectory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData + " C1", parentDirectoryId);
        var childDirectory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData + " C2", childDirectory1Id);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, childDirectory2Id);
        await app.TestData.PageDirectories().AddAccessRuleAsync(childDirectory2Id, TestUserArea1.Code);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        await contentRepository
            .PageDirectories()
            .DeleteAsync(parentDirectoryId);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var parentDirectoryExists = await DoesDirectoryExistsAsync(parentDirectoryId);
        var childDirectory1Exists = await DoesDirectoryExistsAsync(childDirectory1Id);
        var childDirectory2Exists = await DoesDirectoryExistsAsync(childDirectory2Id);
        var pageExists = await dbContext
            .Pages
            .AsNoTracking()
            .FilterById(pageId)
            .AnyAsync();

        using (new AssertionScope())
        {
            parentDirectoryExists.Should().BeFalse();
            childDirectory1Exists.Should().BeFalse();
            childDirectory2Exists.Should().BeFalse();
            pageExists.Should().BeFalse();
        }
    }

    [Fact]
    public async Task WhenRoot_Throws()
    {
        using var app = _appFactory.Create();
        var rootDirectoryId = app.SeededEntities.RootDirectoryId;
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        await contentRepository
            .Awaiting(r => r.PageDirectories().DeleteAsync(rootDirectoryId))
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMemberNames("PageDirectoryId");
    }

    [Fact]
    public async Task WhenRelatedDataDependency_Cascades()
    {
        var uniqueData = UNIQUE_PREFIX + "RelDep";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.UnstructuredData().AddAsync<PageDirectoryEntityDefinition>(directoryId, RelatedEntityCascadeAction.Cascade);

        await contentRepository
            .PageDirectories()
            .DeleteAsync(directoryId);

        var dependency = await app.TestData.UnstructuredData().GetAsync<PageDirectoryEntityDefinition>(directoryId);

        dependency.Should().BeNull();
    }

    [Fact]
    public async Task WhenRequiredRelatedDataDependency_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + "ReqRelDep";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.UnstructuredData().AddAsync<PageDirectoryEntityDefinition>(directoryId, RelatedEntityCascadeAction.None);

        await contentRepository
            .Awaiting(r => r.PageDirectories().DeleteAsync(directoryId))
            .Should()
            .ThrowAsync<RequiredDependencyConstaintViolationException>()
            .WithMessage($"Cannot delete * Page Directory * {TestCustomEntityDefinition.EntityName} '{app.SeededEntities.CustomEntityForUnstructuredDataTests.Title}' * dependency*");
    }

    [Fact]
    public async Task WhenNestedRelatedDataDependency_Cascades()
    {
        var uniqueData = UNIQUE_PREFIX + "NestedRelDep";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory1Id);
        var directory3Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory2Id);
        await app.TestData.UnstructuredData().AddAsync<PageDirectoryEntityDefinition>(directory3Id, RelatedEntityCascadeAction.Cascade);

        await contentRepository
            .PageDirectories()
            .DeleteAsync(directory1Id);

        var dependency = await app.TestData.UnstructuredData().GetAsync<PageDirectoryEntityDefinition>(directory3Id);

        dependency.Should().BeNull();
    }

    [Fact]
    public async Task WhenNestedRequiredRelatedDataDependency_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + "NestedReqRelDep";
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory1Id);
        var directory3Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory2Id);
        await app.TestData.UnstructuredData().AddAsync<PageDirectoryEntityDefinition>(directory3Id, RelatedEntityCascadeAction.None);

        await contentRepository
            .Awaiting(r => r.PageDirectories().DeleteAsync(directory1Id))
            .Should()
            .ThrowAsync<RequiredDependencyConstaintViolationException>()
            .WithMessage($"Cannot delete * Page Directory * {TestCustomEntityDefinition.EntityName} '{app.SeededEntities.CustomEntityForUnstructuredDataTests.Title}' * dependency*");
    }

    [Fact]
    public async Task WhenIndirectlyRelatedDataDependency_Cascades()
    {
        var uniqueData = UNIQUE_PREFIX + "IndirectlyRelDep";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory1Id);
        var directory3Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory2Id);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directory3Id, c => { c.Publish = true; });
        await app.TestData.UnstructuredData().AddAsync<PageEntityDefinition>(pageId, RelatedEntityCascadeAction.Cascade);

        await contentRepository
            .PageDirectories()
            .DeleteAsync(directory1Id);

        var dependency = await app.TestData.UnstructuredData().GetAsync<PageEntityDefinition>(pageId);

        dependency.Should().BeNull();
    }

    [Fact]
    public async Task WhenRequiredIndirectlyRelatedDataDependency_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + "ReqIndirectlyRelDep";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory1Id);
        var directory3Id = await app.TestData.PageDirectories().AddAsync(uniqueData, directory2Id);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directory3Id, c => { c.Publish = true; });
        await app.TestData.UnstructuredData().AddAsync<PageEntityDefinition>(pageId, RelatedEntityCascadeAction.None);

        await contentRepository
            .Awaiting(r => r.PageDirectories().DeleteAsync(directory1Id))
            .Should()
            .ThrowAsync<RequiredDependencyConstaintViolationException>()
            .WithMessage($"Cannot delete * Page Directory * {TestCustomEntityDefinition.EntityName} '{app.SeededEntities.CustomEntityForUnstructuredDataTests.Title}' * dependency*");
    }

    [Fact]
    public async Task WhenDeleted_SendsMessages()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenDeleted_SendsMessages);
        var uniqueDataSlug = SlugFormatter.ToSlug(uniqueData);
        using var app = _appFactory.Create();
        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var directory2Id = await app.TestData.PageDirectories().AddAsync("2", directory1Id);
        var directory3Id = await app.TestData.PageDirectories().AddAsync("3", directory2Id);
        var directory1PageId = await app.TestData.Pages().AddAsync("d1p1", directory1Id, c => c.Publish = true);
        var directory3Page1Id = await app.TestData.Pages().AddAsync("d3p1", directory3Id, c => c.Publish = true);
        var directory3Page2Id = await app.TestData.Pages().AddAsync("d3p2", directory3Id, c => c.Publish = true);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .PageDirectories()
            .DeleteAsync(directory1Id);

        var directoryMessages = new Dictionary<int, string>()
        {
            { directory1Id, $"/{uniqueDataSlug}" },
            { directory2Id, $"/{uniqueDataSlug}/2" },
            { directory3Id, $"/{uniqueDataSlug}/2/3" },
        };

        var pageMessages = new Dictionary<int, string>()
        {
            { directory1PageId, $"/{uniqueDataSlug}/d1p1" },
            { directory3Page1Id, $"/{uniqueDataSlug}/2/3/d3p1" },
            { directory3Page2Id, $"/{uniqueDataSlug}/2/3/d3p2" },
        };

        using (new AssertionScope())
        {
            foreach (var message in pageMessages)
            {
                app.Mocks
                    .CountMessagesPublished<PageDeletedMessage>(m =>
                    {
                        return m.PageId == message.Key && m.FullUrlPath == message.Value;
                    })
                    .Should().Be(1);
            }
            foreach (var message in directoryMessages)
            {
                app.Mocks
                    .CountMessagesPublished<PageDirectoryDeletedMessage>(m =>
                    {
                        return m.PageDirectoryId == message.Key && m.FullUrlPath == message.Value;
                    })
                    .Should().Be(1);
            }
        }
    }

    private async Task<bool> DoesDirectoryExistsAsync(int directoryId)
    {
        using var app = _appFactory.Create();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var exists = await dbContext
            .PageDirectories
            .AsNoTracking()
            .FilterById(directoryId)
            .AnyAsync();

        return exists;
    }
}
