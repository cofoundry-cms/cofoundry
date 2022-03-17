using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddPageCommandHandlerTests
{
    const string UNIQUE_PREFIX = "AddPageCHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public AddPageCommandHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task WhenMinimalData_Adds()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenMinimalData_Adds);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var page = await dbContext
            .Pages
            .Include(p => p.PageVersions)
            .FilterActive()
            .FilterById(addPageCommand.OutputPageId)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        var pageVersion = page.PageVersions.FirstOrDefault();

        using (new AssertionScope())
        {
            addPageCommand.OutputPageId.Should().BePositive();
            page.Should().NotBeNull();
            page.PageId.Should().Be(addPageCommand.OutputPageId);
            page.UrlPath.Should().Be(SlugFormatter.ToSlug(uniqueData));
            page.PageDirectoryId.Should().Be(addPageCommand.PageDirectoryId);
            page.LocaleId.Should().BeNull();
            page.PageTypeId.Should().Be((int)PageType.Generic);
            page.PublishDate.Should().BeNull();
            page.LastPublishDate.Should().BeNull();
            page.PublishStatusCode.Should().Be(PublishStatusCode.Unpublished);
            page.CreateDate.Should().NotBeDefault();
            page.PageVersions.Should().HaveCount(1);

            pageVersion.Should().NotBeNull();
            pageVersion.Title.Should().Be(addPageCommand.Title);
            pageVersion.PageTemplateId.Should().Be(addPageCommand.PageTemplateId);
            pageVersion.DisplayVersion.Should().Be(1);
            pageVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Draft);
            pageVersion.PageVersionId.Should().BePositive();
            pageVersion.MetaDescription.Should().BeEmpty();
            pageVersion.OpenGraphDescription.Should().BeNull();
            pageVersion.OpenGraphImageId.Should().BeNull();
            pageVersion.OpenGraphTitle.Should().BeNull();
            pageVersion.ExcludeFromSitemap.Should().Be(!addPageCommand.ShowInSiteMap);
            pageVersion.CreateDate.Should().NotBeDefault();
        }
    }

    [Fact]
    public async Task WithMetaData_Adds()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WithMetaData_Adds);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
        addPageCommand.MetaDescription = "Test Meta Description";
        addPageCommand.OpenGraphDescription = "Test Open Graph Description";
        addPageCommand.OpenGraphImageId = app.SeededEntities.TestImageId;
        addPageCommand.OpenGraphTitle = "Test Open Graph Title";
        addPageCommand.ShowInSiteMap = true;

        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.PageVersions)
            .FilterActive()
            .FilterById(addPageCommand.OutputPageId)
            .SingleOrDefaultAsync();

        var pageVersion = page.PageVersions.FirstOrDefault();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            pageVersion.Should().NotBeNull();
            pageVersion.MetaDescription.Should().Be(addPageCommand.MetaDescription);
            pageVersion.OpenGraphDescription.Should().Be(addPageCommand.OpenGraphDescription);
            pageVersion.OpenGraphImageId.Should().Be(addPageCommand.OpenGraphImageId);
            pageVersion.OpenGraphTitle.Should().Be(addPageCommand.OpenGraphTitle);
            pageVersion.ExcludeFromSitemap.Should().Be(!addPageCommand.ShowInSiteMap);
        }
    }

    [Fact]
    public async Task WithTags_Adds()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WithTags_Adds);
        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
        addPageCommand.Tags.Add(app.SeededEntities.TestTag.TagText);
        addPageCommand.Tags.Add(uniqueData);

        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.PageTags)
            .ThenInclude(pt => pt.Tag)
            .FilterActive()
            .FilterById(addPageCommand.OutputPageId)
            .SingleOrDefaultAsync();

        var testTag = page.PageTags.Select(t => t.TagId == app.SeededEntities.TestTag.TagId);
        var uniqueTag = page.PageTags.Select(t => t.Tag.TagText == uniqueData);

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            page.PageTags.Should().HaveCount(2);
            testTag.Should().NotBeNull();
            uniqueTag.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task WhenPublished_SetsPublishedWithCurrentDate()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenPublished_SetsPublishedWithCurrentDate);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var now = new DateTime(2021, 09, 15, 13, 47, 30, DateTimeKind.Utc);
        app.Mocks.MockDateTime(now);

        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
        addPageCommand.Publish = true;

        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.PageVersions)
            .FilterActive()
            .FilterById(addPageCommand.OutputPageId)
            .SingleOrDefaultAsync();

        var pageVersion = page.PageVersions.FirstOrDefault();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            page.PublishDate.Should().Be(now);
            page.LastPublishDate.Should().Be(now);
            page.PublishStatusCode.Should().Be(PublishStatusCode.Published);
            pageVersion.Should().NotBeNull();
            pageVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Published);
        }
    }

    [Fact]
    public async Task WhenPublishedWithDate_SetsPublishedWithSpecifiedDate()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenPublishedWithDate_SetsPublishedWithSpecifiedDate);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var now = new DateTime(2021, 09, 15, 13, 47, 30, DateTimeKind.Utc);
        app.Mocks.MockDateTime(now);
        var publishDate = new DateTime(2031, 02, 11, 05, 32, 28, DateTimeKind.Utc);

        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
        addPageCommand.Publish = true;
        addPageCommand.PublishDate = publishDate;

        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var page = await dbContext
            .Pages
            .AsNoTracking()
            .Include(p => p.PageVersions)
            .FilterActive()
            .FilterById(addPageCommand.OutputPageId)
            .SingleOrDefaultAsync();

        var pageVersion = page.PageVersions.FirstOrDefault();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            page.PublishDate.Should().Be(publishDate);
            page.LastPublishDate.Should().Be(publishDate);
            page.PublishStatusCode.Should().Be(PublishStatusCode.Published);
            pageVersion.Should().NotBeNull();
            pageVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Published);
        }
    }

    [Fact]
    public async Task WithCustomEntityTemplate_Adds()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WithCustomEntityTemplate_Adds);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var addPageCommand = app.TestData.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var page = await dbContext
            .Pages
            .AsNoTracking()
            .FilterActive()
            .FilterById(addPageCommand.OutputPageId)
            .SingleOrDefaultAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            page.PageTypeId.Should().Be((int)PageType.CustomEntityDetails);
            page.CustomEntityDefinitionCode.Should().Be(TestCustomEntityDefinition.Code);
            page.UrlPath.Should().Be(addPageCommand.CustomEntityRoutingRule);
        }
    }

    [Fact]
    public async Task CanAddMultipleToOneDirectory()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanAddMultipleToOneDirectory);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.Pages().AddAsync(uniqueData + " 1", directoryId);
        await app.TestData.Pages().AddAsync(uniqueData + " 2", directoryId);
        await app.TestData.Pages().AddAsync(uniqueData + " 3", directoryId);
        var addPageCommand = app.TestData.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
        var pages = await dbContext
            .Pages
            .AsNoTracking()
            .FilterActive()
            .FilterByPageDirectoryId(directoryId)
            .ToListAsync();

        pages.Should().HaveCount(4);
    }

    [Fact]
    public async Task WhenDuplicatePath_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenDuplicatePath_Throws);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.Pages().AddAsync(uniqueData, directoryId);
        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Awaiting(r => r.Pages().AddAsync(addPageCommand))
            .Should()
            .ThrowAsync<UniqueConstraintViolationException>()
            .WithMemberNames(nameof(addPageCommand.UrlPath));
    }

    [Fact]
    public async Task WhenCustomEntityPageTypeWithGenericTemplate_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenCustomEntityPageTypeWithGenericTemplate_Throws);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var addPageCommand = app.TestData.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);
        addPageCommand.PageTemplateId = app.SeededEntities.TestPageTemplate.PageTemplateId;

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Awaiting(r => r.Pages().AddAsync(addPageCommand))
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMemberNames(nameof(addPageCommand.PageTemplateId));
    }

    [Fact]
    public async Task WhenGenericPageTypeWithCustomEntityTemplate_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenGenericPageTypeWithCustomEntityTemplate_Throws);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
        addPageCommand.PageTemplateId = app.SeededEntities.TestCustomEntityPageTemplate.PageTemplateId;

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Awaiting(r => r.Pages().AddAsync(addPageCommand))
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMemberNames(nameof(addPageCommand.PageTemplateId));
    }

    [Fact]
    public async Task WhenArchivedPageTemplate_Throws()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenArchivedPageTemplate_Throws);

        using var app = _appFactory.Create();
        var pageTemplateId = await app.TestData.PageTemplates().AddMockTemplateAsync(uniqueData);
        await app.TestData.PageTemplates().ArchiveTemplateAsync(pageTemplateId);
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);

        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
        addPageCommand.PageTemplateId = pageTemplateId;

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Awaiting(r => r.Pages().AddAsync(addPageCommand))
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMemberNames(nameof(addPageCommand.PageTemplateId));
    }

    [Fact]
    public async Task WhenNotPublished_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenNotPublished_SendsMessage);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        app.Mocks
            .CountMessagesPublished<PageAddedMessage>(m => m.PageId == addPageCommand.OutputPageId && !m.HasPublishedVersionChanged)
            .Should().Be(1);
    }

    [Fact]
    public async Task WhenPublished_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(WhenPublished_SendsMessage);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
        addPageCommand.Publish = true;

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        app.Mocks
            .CountMessagesPublished<PageAddedMessage>(m => m.PageId == addPageCommand.OutputPageId && m.HasPublishedVersionChanged)
            .Should().Be(1);
    }
}
