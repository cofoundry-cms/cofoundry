using Cofoundry.Core;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageRenderDetailsByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageRenderDetailsByIdCHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageRenderDetailsByIdQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Theory]
        [InlineData(PublishStatusQuery.Draft, WorkFlowStatus.Draft)]
        [InlineData(PublishStatusQuery.Latest, WorkFlowStatus.Draft)]
        [InlineData(PublishStatusQuery.PreferPublished, WorkFlowStatus.Draft)]
        [InlineData(PublishStatusQuery.Published, null)]
        public async Task WhenDraftOnlyQueriedWithPublishStatus_ReturnsVersionWithWorkflowStatus(PublishStatusQuery publishStatus, WorkFlowStatus? workFlowStatus)
        {
            var uniqueData = UNIQUE_PREFIX + "DraftOnlyQPubStatus_" + publishStatus;

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);

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

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

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
        public async Task WhenPublishedWithDraftQueriedWithPublishStatus_ReturnsVersionWithWorkflowStatus(PublishStatusQuery publishStatus, WorkFlowStatus? workFlowStatus)
        {
            var uniqueData = UNIQUE_PREFIX + "PubDraftQPubStatus_" + publishStatus;

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            var versionId = await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().PublishAsync(pageId);

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
                    page.PageVersionId.Should().Be(versionId);
                    page.WorkFlowStatus.Should().Be(workFlowStatus);
                }
            }
        }

        [Fact]
        public async Task WhenSpecificVersion_ReturnsVersion()
        {
            var uniqueData = UNIQUE_PREFIX + "SpecificVer_RetVer";

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            var versionToQueryId = await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().PublishAsync(pageId);
            await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().PublishAsync(pageId);
            var latestVersionId = await _testDataHelper.Pages().AddDraftAsync(pageId);

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

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var page1Id = await _testDataHelper.Pages().AddAsync(uniqueData + "1", directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddDraftAsync(page1Id);

            var page2Id = await _testDataHelper.Pages().AddAsync(uniqueData + "2", directoryId, c => c.Publish = true);
            var versionId = await _testDataHelper.Pages().AddDraftAsync(page2Id);

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

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

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
        public async Task MapsBasicData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(MapsBasicData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.Tags.Add(_dbDependentFixture.SeededEntities.TestTag.TagText);
            addPageCommand.OpenGraphTitle = uniqueData + "OG Title";
            addPageCommand.OpenGraphDescription = uniqueData + "OG desc";
            addPageCommand.OpenGraphImageId = _dbDependentFixture.SeededEntities.TestImageId;
            addPageCommand.MetaDescription = uniqueData + "Meta";
            addPageCommand.Publish = true;
            addPageCommand.ShowInSiteMap = true;

            var pageId = await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var versionId = await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().PublishAsync(pageId);

            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRenderDetails()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.PageId.Should().Be(pageId);
                page.PageVersionId.Should().Be(versionId);
                page.CreateDate.Should().NotBeDefault();

                page.PageRoute.Should().NotBeNull();
                page.PageRoute.PageId.Should().Be(pageId);

                page.Should().NotBeNull();
                page.OpenGraph.Should().NotBeNull();
                page.OpenGraph.Title.Should().Be(addPageCommand.OpenGraphTitle);
                page.OpenGraph.Description.Should().Be(addPageCommand.OpenGraphDescription);
                page.OpenGraph.Image.Should().NotBeNull();
                page.OpenGraph.Image.ImageAssetId.Should().Be(addPageCommand.OpenGraphImageId);
                page.MetaDescription.Should().Be(addPageCommand.MetaDescription);

                page.Title.Should().Be(addPageCommand.Title);
                page.Template.Should().NotBeNull();
                page.Template.PageTemplateId.Should().Be(addPageCommand.PageTemplateId);
                page.PageVersionId.Should().Be(versionId);
                page.WorkFlowStatus.Should().Be(WorkFlowStatus.Published);
                page.Regions.Should().NotBeNull();
                page.Regions.Should().HaveCount(1);
            }
        }
    }
}
