namespace Cofoundry.Domain.Tests.Integration.Pages.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class SearchPageSummariesQueryHandlerTests
{
    const string UNIQUE_PREFIX = "SearchPageDetailsSumQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public SearchPageSummariesQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
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
            .AsSummaries(new SearchPageSummariesQuery()
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
            .AsSummaries(new SearchPageSummariesQuery()
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
            .AsSummaries(new SearchPageSummariesQuery()
            {
                PageDirectoryId = directoryId
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            var page = result.Items.SingleOrDefault(p => p.PageId == pageId);
            GetPageSummariesByIdRangeQueryHandlerTests.AssertBasicDataMapping(uniqueData, addPageCommand, page);
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
            .AsSummaries(new SearchPageSummariesQuery())
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
            .AsSummaries(new SearchPageSummariesQuery()
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
    public async Task CanSearchByPageTemplate()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByPageTemplate);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var page1Id = await app.TestData.Pages().AddAsync(uniqueData + "1", directoryId, c => c.Publish = true);
        var page2Id = await app.TestData.Pages().AddCustomEntityPageDetailsAsync(uniqueData + "2", directoryId, c => c.Publish = true);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsSummaries(new SearchPageSummariesQuery()
            {
                PageDirectoryId = directoryId,
                PageTemplateId = app.SeededEntities.TestPageTemplate.PageTemplateId
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.Single().PageId.Should().Be(page1Id);
        }
    }

    [Fact]
    public async Task CanSearchByTag()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByTag);

        using var app = _appFactory.Create();
        var tag1 = UNIQUE_PREFIX.Trim() + "s1";
        var tag2 = UNIQUE_PREFIX.Trim() + "s2";
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var page1Id = await app.TestData.Pages().AddAsync(uniqueData + "1", directoryId, c =>
        {
            c.Publish = true;
            c.Tags = new string[] { tag1 };
        });
        var page2Id = await app.TestData.Pages().AddAsync(uniqueData + "2", directoryId, c =>
        {
            c.Publish = true;
            c.Tags = new string[] { tag1, tag2 };
        });
        var page3Id = await app.TestData.Pages().AddAsync(uniqueData + "3", directoryId, c =>
        {
            c.Publish = true;
            c.Tags = new string[] { app.SeededEntities.TestTag.TagText };
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsSummaries(new SearchPageSummariesQuery()
            {
                PageDirectoryId = directoryId,
                Tags = $"{tag1}, {tag2}"
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.Should().Contain(p => p.PageId == page2Id);
        }
    }

    [Fact]
    public async Task CanSearchByTextUsingTitle()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByTextUsingTitle);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var page1Id = await app.TestData.Pages().AddAsync("Red Pippin", directoryId, c =>
        {
            c.Publish = true;
            c.UrlPath = "1";
        });
        var page2Id = await app.TestData.Pages().AddAsync("Ribston Pippin", directoryId, c =>
        {
            c.Publish = true;
            c.UrlPath = "2";
        });
        var page3Id = await app.TestData.Pages().AddAsync("Red Prince", directoryId, c =>
        {
            c.Publish = true;
            c.UrlPath = "3";
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsSummaries(new SearchPageSummariesQuery()
            {
                PageDirectoryId = directoryId,
                Text = "pip"
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Should().Contain(p => p.PageId == page1Id);
            result.Items.Should().Contain(p => p.PageId == page2Id);
        }
    }

    [Fact]
    public async Task CanSearchByTextUsingUrlSlug()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByTextUsingUrlSlug);

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var page1Id = await app.TestData.Pages().AddAsync("Red Pippin", directoryId, c =>
        {
            c.Publish = true;
            c.Title = "1";
        });
        var page2Id = await app.TestData.Pages().AddAsync("Ribston Pippin", directoryId, c =>
        {
            c.Publish = true;
            c.Title = "2";
        });
        var page3Id = await app.TestData.Pages().AddAsync("Red Prince", directoryId, c =>
        {
            c.Publish = true;
            c.Title = "3";
        });

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var result = await contentRepository
            .Pages()
            .Search()
            .AsSummaries(new SearchPageSummariesQuery()
            {
                PageDirectoryId = directoryId,
                Text = "-pip"
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Should().Contain(p => p.PageId == page1Id);
            result.Items.Should().Contain(p => p.PageId == page2Id);
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
            .AsSummaries(new SearchPageSummariesQuery()
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
            .AsSummaries(new SearchPageSummariesQuery()
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
            result.Items.Should().BeInAscendingOrder(p => p.AuditData.CreateDate);
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
            .AsSummaries(new SearchPageSummariesQuery()
            {
                PageDirectoryId = directoryId,
                SortBy = PageQuerySortType.PublishDate
            })
            .ExecuteAsync();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.Items.Should().BeInDescendingOrder(p => p.PublishDate);
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
            .AsSummaries(new SearchPageSummariesQuery()
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
