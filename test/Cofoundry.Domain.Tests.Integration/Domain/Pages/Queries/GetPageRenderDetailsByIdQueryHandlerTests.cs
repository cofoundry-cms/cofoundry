using Cofoundry.Domain.Tests.Shared.Assertions;
using Cofoundry.Web;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetPageRenderDetailsByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageRenderDetailsByIdQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetPageRenderDetailsByIdQueryHandlerTests(
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
                .AsRenderDetails(publishStatus)
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
                .AsRenderDetails(publishStatus)
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
                .AsRenderDetails(publishStatus)
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
                .AsRenderDetails(versionToQueryId)
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
                .AsRenderDetails(versionId)
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
                .AsRenderDetails()
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
                .AsRenderDetails()
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
                .AsRenderDetails()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                AssertBasicDataMapping(addPageCommand, versionId, page);
            }
        }

        [Fact]
        public async Task MapsBlocks()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(MapsBlocks);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            var pageVersionId = await app.TestData.Pages().AddDraftAsync(pageId);
            var plainTextBlockId = await app.TestData.Pages().AddPlainTextBlockToTestTemplateAsync(pageVersionId, uniqueData);
            var imageBlockId = await app.TestData.Pages().AddImageTextBlockToTestTemplateAsync(pageVersionId);
            await app.TestData.Pages().PublishAsync(pageId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderDetails()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                AssertBlockDataMapping(uniqueData, plainTextBlockId, imageBlockId, page);
            }
        }

        /// <summary>
        /// Shared basic data mapping assertions between <see cref="PageRenderDetails"/> 
        /// query handler tests, which are expected to be using the same page
        /// construction schema.
        /// </summary>
        internal static void AssertBasicDataMapping(
            AddPageCommand addPageCommand,
            int versionId,
            PageRenderDetails page
            )
        {
            GetPageRenderSummaryByIdQueryHandlerTests.AssertBasicDataMapping(addPageCommand, versionId, page);

            page.Template.Should().NotBeNull();
            page.Regions.Should().NotBeNull();
            page.Regions.Should().HaveCount(1);
        }

        /// <summary>
        /// Shared block data mappingassertions between <see cref="PageRenderDetails"/> 
        /// query handler tests, which are epxected to be using the same page
        /// and block data construction schema.
        /// </summary>
        internal static void AssertBlockDataMapping(
            string uniqueData,
            int plainTextBlockId,
            int imageBlockId,
            PageRenderDetails page
            )
        {
            page.Should().NotBeNull();

            page.Regions.Should().NotBeNull();
            page.Regions.Should().HaveCount(1);

            var bodyRegion = page.Regions.Single();
            bodyRegion.Name.Should().Be("Body");
            bodyRegion.PageTemplateRegionId.Should().BePositive();
            bodyRegion.Blocks.Should().HaveCount(2);

            var plainTextBlock = bodyRegion.Blocks.SingleOrDefault(b => b.PageVersionBlockId == plainTextBlockId);
            plainTextBlock.BlockType.Should().NotBeNull();
            plainTextBlock.BlockType.FileName.Should().Be("PlainText");
            plainTextBlock.DisplayModel.Should().NotBeNull();
            plainTextBlock.DisplayModel.Should().BeOfType<PlainTextDataModel>();
            var plainTextDisplayModel = plainTextBlock.DisplayModel as PlainTextDataModel;
            plainTextDisplayModel.PlainText.Should().Be(uniqueData);
            plainTextBlock.EntityVersionPageBlockId.Should().Be(plainTextBlockId);
            plainTextBlock.Template.Should().BeNull();

            var imageBlock = bodyRegion.Blocks.SingleOrDefault(b => b.PageVersionBlockId == imageBlockId);
            imageBlock.BlockType.Should().NotBeNull();
            imageBlock.BlockType.FileName.Should().Be("Image");
            imageBlock.DisplayModel.Should().NotBeNull();
            imageBlock.DisplayModel.Should().BeOfType<ImageDisplayModel>();
            var imageDisplayModel = imageBlock.DisplayModel as ImageDisplayModel;
            imageDisplayModel.AltText.Should().NotBeNull();
            imageDisplayModel.Source.Should().NotBeNull();
            imageBlock.EntityVersionPageBlockId.Should().Be(imageBlockId);
            imageBlock.Template.Should().BeNull();
        }
    }
}
