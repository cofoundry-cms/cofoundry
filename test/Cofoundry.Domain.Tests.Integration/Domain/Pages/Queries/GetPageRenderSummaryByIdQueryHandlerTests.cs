using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetPageRenderSummaryByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageRenderSummaryByIdQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetPageRenderSummaryByIdQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Theory]
        [InlineData(PublishStatusQuery.Draft, WorkFlowStatus.Draft)]
        [InlineData(PublishStatusQuery.Latest, WorkFlowStatus.Draft)]
        [InlineData(PublishStatusQuery.PreferPublished, WorkFlowStatus.Draft)]
        [InlineData(PublishStatusQuery.Published, null)]
        public async Task WhenDraftOnlyQueriedWithPublishStatus_ReturnsVersionWithWorkflowStatus(PublishStatusQuery publishStatus, WorkFlowStatus? workFlowStatus)
        {
            var uniqueData = UNIQUE_PREFIX + "DraftOnlyQPubStatus_" + publishStatus;

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderSummary(publishStatus)
                .ExecuteAsync();

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

        [Theory]
        [InlineData(PublishStatusQuery.Draft, null)]
        [InlineData(PublishStatusQuery.Latest, WorkFlowStatus.Published)]
        [InlineData(PublishStatusQuery.PreferPublished, WorkFlowStatus.Published)]
        [InlineData(PublishStatusQuery.Published, WorkFlowStatus.Published)]
        public async Task WhenPublishedOnlyQueriedWithPublishStatus_ReturnsVersionWithWorkflowStatus(PublishStatusQuery publishStatus, WorkFlowStatus? workFlowStatus)
        {
            var uniqueData = UNIQUE_PREFIX + "PubOnlyQPubStatus_" + publishStatus;

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderSummary(publishStatus)
                .ExecuteAsync();

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

        [Theory]
        [InlineData(PublishStatusQuery.Draft, WorkFlowStatus.Draft)]
        [InlineData(PublishStatusQuery.Latest, WorkFlowStatus.Draft)]
        [InlineData(PublishStatusQuery.PreferPublished, WorkFlowStatus.Published)]
        [InlineData(PublishStatusQuery.Published, WorkFlowStatus.Published)]
        public async Task WhenPublishedWithDraftQueriedWithPublishStatus_ReturnsVersionWithWorkflowStatus(PublishStatusQuery publishStatus, WorkFlowStatus workFlowStatus)
        {
            var uniqueData = UNIQUE_PREFIX + "PubDraftQPubStatus_" + publishStatus;

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            var publishedVersionId = await app.TestData.Pages().AddDraftAsync(pageId);
            await app.TestData.Pages().PublishAsync(pageId);
            var draftVersionId = await app.TestData.Pages().AddDraftAsync(pageId);
            var expectedVersionId = workFlowStatus == WorkFlowStatus.Draft ? draftVersionId : publishedVersionId;

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderSummary(publishStatus)
                .ExecuteAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.PageId.Should().Be(pageId);
                page.PageVersionId.Should().Be(expectedVersionId);
                page.WorkFlowStatus.Should().Be(workFlowStatus);
            }
        }

        [Fact]
        public async Task WhenSpecificVersion_ReturnsVersion()
        {
            var uniqueData = UNIQUE_PREFIX + "SpecificVer_RetVer";

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            var versionToQueryId = await app.TestData.Pages().AddDraftAsync(pageId);
            await app.TestData.Pages().PublishAsync(pageId);
            await app.TestData.Pages().AddDraftAsync(pageId);
            await app.TestData.Pages().PublishAsync(pageId);
            var latestVersionId = await app.TestData.Pages().AddDraftAsync(pageId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderSummary(versionToQueryId)
                .ExecuteAsync();

            using (new AssertionScope())
            {
                versionToQueryId.Should().NotBe(latestVersionId);
                page.Should().NotBeNull();
                page.PageId.Should().Be(pageId);
                page.PageVersionId.Should().Be(versionToQueryId);
                page.WorkFlowStatus.Should().Be(WorkFlowStatus.Published);
            }
        }

        [Fact]
        public async Task WhenSpecificVersionInvalid_ReturnsNull()
        {
            var uniqueData = UNIQUE_PREFIX + "SpecificVerInvalid_RetNull";

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var page1Id = await app.TestData.Pages().AddAsync(uniqueData + "1", directoryId, c => c.Publish = true);
            await app.TestData.Pages().AddDraftAsync(page1Id);

            var page2Id = await app.TestData.Pages().AddAsync(uniqueData + "2", directoryId, c => c.Publish = true);
            var versionId = await app.TestData.Pages().AddDraftAsync(page2Id);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetById(page1Id)
                .AsRenderSummary(versionId)
                .ExecuteAsync();

            page.Should().BeNull();
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

            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderSummary()
                .ExecuteAsync();

            page.Should().BeNull();
        }

        [Fact]
        public async Task DoesNotReturnPageWithArchivedTemplate()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(DoesNotReturnPageWithArchivedTemplate);

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
            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderSummary()
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
            await app.TestData.Pages().PublishAsync(pageId);

            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderSummary()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                AssertBasicDataMapping(addPageCommand, versionId, page);
            }
        }

        /// <summary>
        /// Shared basic data mapping assertions between <see cref="PageRenderSummary"/> 
        /// query handler tests, which are expected to be using the same page
        /// construction schema.
        /// </summary>
        internal static void AssertBasicDataMapping(
            AddPageCommand addPageCommand,
            int versionId,
            PageRenderSummary page
            )
        {
            page.Should().NotBeNull();
            page.PageId.Should().Be(addPageCommand.OutputPageId);
            page.PageVersionId.Should().Be(versionId);
            page.CreateDate.Should().NotBeDefault();

            page.PageRoute.Should().NotBeNull();
            page.PageRoute.PageId.Should().Be(addPageCommand.OutputPageId);

            page.Should().NotBeNull();
            page.OpenGraph.Should().NotBeNull();
            page.OpenGraph.Title.Should().Be(addPageCommand.OpenGraphTitle);
            page.OpenGraph.Description.Should().Be(addPageCommand.OpenGraphDescription);
            page.OpenGraph.Image.Should().NotBeNull();
            page.OpenGraph.Image.ImageAssetId.Should().Be(addPageCommand.OpenGraphImageId);
            page.MetaDescription.Should().Be(addPageCommand.MetaDescription);

            page.Title.Should().Be(addPageCommand.Title);
            page.PageVersionId.Should().Be(versionId);
            page.WorkFlowStatus.Should().Be(WorkFlowStatus.Published);
        }
    }
}
