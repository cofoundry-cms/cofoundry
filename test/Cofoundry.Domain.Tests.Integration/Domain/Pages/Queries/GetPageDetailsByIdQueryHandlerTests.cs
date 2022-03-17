namespace Cofoundry.Domain.Tests.Integration.Pages.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class GetPageDetailsByIdQueryHandlerTests
{
    const string UNIQUE_PREFIX = "GPageDetailsByIdQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public GetPageDetailsByIdQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
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
        var page = await contentRepository
            .Pages()
            .GetById(pageId)
            .AsDetails()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            page.PageId.Should().Be(pageId);
            page.PageRoute.Should().NotBeNull();
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

        var page = await contentRepository
            .Pages()
            .GetById(pageId)
            .AsDetails()
            .ExecuteAsync();

        page.Should().BeNull();
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

        var page = await contentRepository
            .Pages()
            .GetById(pageId)
            .AsDetails()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            page.Should().NotBeNull();
            page.PageId.Should().Be(pageId);

            page.AuditData.Should().NotBeNull();
            page.AuditData.Creator.Should().NotBeNull();
            page.AuditData.Creator.UserId.Should().BePositive();

            page.PageRoute.Should().NotBeNull();
            page.PageRoute.PageId.Should().Be(pageId);

            page.Tags.Should().NotBeNull();
            page.Tags.Should().HaveCount(1);
            page.Tags.Single().Should().Be(app.SeededEntities.TestTag.TagText);

            page.LatestVersion.Should().NotBeNull();
            page.LatestVersion.OpenGraph.Should().NotBeNull();
            page.LatestVersion.OpenGraph.Title.Should().Be(addPageCommand.OpenGraphTitle);
            page.LatestVersion.OpenGraph.Description.Should().Be(addPageCommand.OpenGraphDescription);
            page.LatestVersion.OpenGraph.Image.Should().NotBeNull();
            page.LatestVersion.OpenGraph.Image.ImageAssetId.Should().Be(addPageCommand.OpenGraphImageId);
            page.LatestVersion.MetaDescription.Should().Be(addPageCommand.MetaDescription);

            page.LatestVersion.AuditData.Should().NotBeNull();
            page.LatestVersion.AuditData.Creator.Should().NotBeNull();
            page.LatestVersion.AuditData.Creator.UserId.Should().BePositive();
            page.LatestVersion.DisplayVersion.Should().Be(2);
            page.LatestVersion.ShowInSiteMap.Should().Be(addPageCommand.ShowInSiteMap);
            page.LatestVersion.Title.Should().Be(addPageCommand.Title);
            page.LatestVersion.Template.Should().NotBeNull();
            page.LatestVersion.Template.PageTemplateId.Should().Be(addPageCommand.PageTemplateId);
            page.LatestVersion.PageVersionId.Should().Be(versionId);
            page.LatestVersion.WorkFlowStatus.Should().Be(WorkFlowStatus.Draft);
            page.LatestVersion.Regions.Should().NotBeNull();
            page.LatestVersion.Regions.Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task WhenPublished_ReturnsLatestPublishedVersion()
    {
        var uniqueData = UNIQUE_PREFIX + "WhenPublished_RetLatestPublishedVer";

        using var app = _appFactory.Create();
        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
        await app.TestData.Pages().AddDraftAsync(pageId);
        await app.TestData.Pages().PublishAsync(pageId);
        var latestVersionId = await app.TestData.Pages().AddDraftAsync(pageId);
        await app.TestData.Pages().PublishAsync(pageId);

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var page = await contentRepository
            .Pages()
            .GetById(pageId)
            .AsDetails()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            page.LatestVersion.Should().NotBeNull();
            page.LatestVersion.WorkFlowStatus.Should().Be(WorkFlowStatus.Published);
            page.LatestVersion.PageVersionId.Should().Be(latestVersionId);
        }
    }
}
