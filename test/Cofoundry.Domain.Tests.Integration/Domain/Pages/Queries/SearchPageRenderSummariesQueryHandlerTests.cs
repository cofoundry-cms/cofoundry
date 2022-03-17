namespace Cofoundry.Domain.Tests.Integration.Pages.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class SearchPageRenderSummariesQueryHandlerTests
{
    const string UNIQUE_PREFIX = "SearchPageRenderSumQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public SearchPageRenderSummariesQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Theory]
    [InlineData(PublishStatusQuery.Draft, WorkFlowStatus.Draft, null, WorkFlowStatus.Draft)]
    [InlineData(PublishStatusQuery.Latest, WorkFlowStatus.Draft, WorkFlowStatus.Published, WorkFlowStatus.Draft)]
    [InlineData(PublishStatusQuery.PreferPublished, WorkFlowStatus.Draft, WorkFlowStatus.Published, WorkFlowStatus.Published)]
    [InlineData(PublishStatusQuery.Published, null, WorkFlowStatus.Published, WorkFlowStatus.Published)]
    public async Task WhenQueriedWithPublishStatus_ReturnsCorrectWorkflowStatus(
        PublishStatusQuery publishStatus,
        WorkFlowStatus? draftOnlyWorkFlowStatus,
        WorkFlowStatus? publishOnlyWorkFlowStatus,
        WorkFlowStatus? publishAndDraftWorkFlowStatus
        )
    {
        var uniqueData = UNIQUE_PREFIX + "QPubStatus_" + publishStatus;

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageWithDraftOnlyId = await app.TestData.Pages().AddAsync(uniqueData + "_D", directoryId);
        var pageWithPublishedOnlyId = await app.TestData.Pages().AddAsync(uniqueData + "_P", directoryId, c => c.Publish = true);
        var pageWithPublishedandDraftId = await app.TestData.Pages().AddAsync(uniqueData + "_DP", directoryId, c => c.Publish = true);
        await app.TestData.Pages().AddDraftAsync(pageWithPublishedandDraftId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var pages = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directoryId,
                PublishStatus = publishStatus
            })
            .ExecuteAsync();

        AssertStatus(pages, pageWithDraftOnlyId, draftOnlyWorkFlowStatus);
        AssertStatus(pages, pageWithPublishedOnlyId, publishOnlyWorkFlowStatus);
        AssertStatus(pages, pageWithPublishedandDraftId, publishAndDraftWorkFlowStatus);

        static void AssertStatus(
            PagedQueryResult<PageRenderSummary> pages,
            int pageId,
            WorkFlowStatus? workFlowStatus
            )
        {
            var page = pages.Items.SingleOrDefault(p => p.PageId == pageId);

            if (!workFlowStatus.HasValue)
            {
                page.Should().BeNull();
            }
            else
            {
                using (new AssertionScope())
                {
                    page.Should().NotBeNull();
                    page.PageId.Should().Be(pageId);
                    page.WorkFlowStatus.Should().Be(workFlowStatus);
                }
            }
        }
    }

    [Fact]
    public async Task WhenSpecificVersion_Throws()
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        await contentRepository
            .Awaiting(r => r
                .Pages()
                .Search()
                .AsRenderSummaries(new SearchPageRenderSummariesQuery()
                {
                    PublishStatus = PublishStatusQuery.SpecificVersion
                })
                .ExecuteAsync()
                )
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{nameof(PublishStatusQuery.SpecificVersion)}*");
    }

    [Fact]
    public async Task DoesNotReturnDeletedPage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(DoesNotReturnDeletedPage);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        await contentRepository
            .Pages()
            .DeleteAsync(pageId);

        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directoryId
            })
            .ExecuteAsync();

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task DoesNotReturnPageWithArchivedTemplate()
    {
        var uniqueData = UNIQUE_PREFIX + "NotRetPageWithArchivedTmpl";

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageTemplateId = await app.TestData.PageTemplates().AddMockTemplateAsync(uniqueData);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c =>
        {
            c.Publish = true;
            c.PageTemplateId = pageTemplateId;
        });
        await app.TestData.PageTemplates().ArchiveTemplateAsync(pageTemplateId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directoryId
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task MapsBasicData()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(MapsBasicData);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
        addPageCommand.Tags.Add(app.SeededEntities.TestTag.TagText);
        addPageCommand.OpenGraphTitle = uniqueData + "OG Title";
        addPageCommand.OpenGraphDescription = uniqueData + "OG desc";
        addPageCommand.OpenGraphImageId = app.SeededEntities.TestImageId;
        addPageCommand.MetaDescription = uniqueData + "Meta";
        addPageCommand.Publish = true;
        addPageCommand.ShowInSiteMap = true;

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var pageId = await contentRepository
            .Pages()
            .AddAsync(addPageCommand);

        var versionId = await app.TestData.Pages().AddDraftAsync(pageId);
        await app.TestData.Pages().PublishAsync(pageId);

        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directoryId
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            var page = result.Items.SingleOrDefault(p => p.PageId == pageId);

            GetPageRenderSummaryByIdQueryHandlerTests.AssertBasicDataMapping(
                addPageCommand,
                versionId,
                page
                );
        }
    }

    [Fact]
    public async Task CanSearch()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSearch);

        using var app = _appFactory.Create();
        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");
        var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "2");
        var page1Id = await app.TestData.Pages().AddAsync(uniqueData + "1", directory1Id, c => c.Publish = true);
        var page2Id = await app.TestData.Pages().AddAsync(uniqueData + "2", directory2Id, c => c.Publish = true);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery())
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Should().HaveCountGreaterOrEqualTo(2);
        }
    }

    [Fact]
    public async Task CanSearchByDirectory()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByDirectory);

        using var app = _appFactory.Create();
        var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");
        var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "2");
        var page1Id = await app.TestData.Pages().AddAsync(uniqueData + "1", directory1Id, c => c.Publish = true);
        var page2Id = await app.TestData.Pages().AddAsync(uniqueData + "2", directory2Id, c => c.Publish = true);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directory2Id
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.Single().PageId.Should().Be(page2Id);
        }
    }

    [Fact]
    public async Task CanPage()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanPage);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.Pages().AddAsync(uniqueData + "1", directoryId, c => c.Publish = true);
        await app.TestData.Pages().AddAsync(uniqueData + "2", directoryId, c => c.Publish = true);
        var page3Id = await app.TestData.Pages().AddAsync(uniqueData + "3", directoryId, c => c.Publish = true);
        var page4Id = await app.TestData.Pages().AddAsync(uniqueData + "4", directoryId, c => c.Publish = true);
        await app.TestData.Pages().AddAsync(uniqueData + "5", directoryId, c => c.Publish = true);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directoryId,
                PageSize = 2,
                PageNumber = 2
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.PageCount.Should().Be(3);
            result.TotalItems.Should().Be(5);
            result.PageSize.Should().Be(2);
            result.PageCount.Should().Be(3);
            result.Items.Should().HaveCount(2);
            result.Items.Should().Contain(p => p.PageId == page3Id);
            result.Items.Should().Contain(p => p.PageId == page4Id);
        }
    }

    [Fact]
    public async Task CanSortByCreateDate()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSortByCreateDate);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.Pages().AddAsync(uniqueData + "T", directoryId, c => c.Publish = true);
        await app.TestData.Pages().AddAsync(uniqueData + "H", directoryId, c => c.Publish = true);
        await app.TestData.Pages().AddAsync(uniqueData + "E", directoryId, c => c.Publish = true);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directoryId,
                SortBy = PageQuerySortType.CreateDate,
                SortDirection = SortDirection.Reversed
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.Items.Should().BeInAscendingOrder(p => p.CreateDate);
        }
    }

    [Fact]
    public async Task CanSortByPublishDate()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSortByPublishDate);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.Pages().AddAsync(uniqueData + "M", directoryId, c => { c.Publish = true; c.PublishDate = new DateTime(2020, 4, 23); });
        await app.TestData.Pages().AddAsync(uniqueData + "E", directoryId, c => { c.Publish = true; c.PublishDate = new DateTime(2021, 2, 18); });
        await app.TestData.Pages().AddAsync(uniqueData + "G", directoryId, c => { c.Publish = true; c.PublishDate = new DateTime(2019, 12, 5); });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directoryId,
                SortBy = PageQuerySortType.PublishDate
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.Items.Should().BeInDescendingOrder(p => p.PageRoute.PublishDate);
        }
    }

    [Fact]
    public async Task CanSortByName()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSortByName);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.Pages().AddAsync(uniqueData + "S", directoryId, c => c.Publish = true);
        await app.TestData.Pages().AddAsync(uniqueData + "P", directoryId, c => c.Publish = true);
        await app.TestData.Pages().AddAsync(uniqueData + "Y", directoryId, c => c.Publish = true);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsRenderSummaries(new SearchPageRenderSummariesQuery()
            {
                PageDirectoryId = directoryId,
                SortBy = PageQuerySortType.Title,
                SortDirection = SortDirection.Reversed
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.Items.Should().BeInDescendingOrder(p => p.Title);
        }
    }
}
