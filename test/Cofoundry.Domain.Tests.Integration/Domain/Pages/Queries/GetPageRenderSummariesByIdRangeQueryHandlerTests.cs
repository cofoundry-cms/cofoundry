using Cofoundry.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageRenderSummariesByIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageRenderSumByIdRangeQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageRenderSummariesByIdRangeQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
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

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageWithDraftOnlyId = await _testDataHelper.Pages().AddAsync(uniqueData + "_D", directoryId);
            var pageWithPublishedOnlyId = await _testDataHelper.Pages().AddAsync(uniqueData + "_P", directoryId, c => c.Publish = true);
            var pageWithPublishedandDraftId = await _testDataHelper.Pages().AddAsync(uniqueData + "_DP", directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddDraftAsync(pageWithPublishedandDraftId);

            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageWithDraftOnlyId, pageWithPublishedOnlyId, pageWithPublishedandDraftId })
                .AsRenderSummaries(publishStatus)
                .ExecuteAsync();

            AssertStatus(pages, pageWithDraftOnlyId, draftOnlyWorkFlowStatus);
            AssertStatus(pages, pageWithPublishedOnlyId, publishOnlyWorkFlowStatus);
            AssertStatus(pages, pageWithPublishedandDraftId, publishAndDraftWorkFlowStatus);

            static void AssertStatus(
                IDictionary<int, PageRenderSummary> pages,
                int pageId,
                WorkFlowStatus? workFlowStatus
                )
            {
                var page = pages.GetOrDefault(pageId);

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
            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var page = await contentRepository
                .Awaiting(r => r
                    .Pages()
                    .GetByIdRange(new int[] { 1 })
                    .AsRenderSummaries(PublishStatusQuery.SpecificVersion)
                    .ExecuteAsync()
                    )
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{nameof(PublishStatusQuery.SpecificVersion)} not supported*");
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

            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageId })
                .AsRenderSummaries()
                .ExecuteAsync();

            pages.Should().NotBeNull();
            pages.Should().BeEmpty();
        }

        [Fact]
        public async Task DoesNotReturnPageWithArchivedTemplate()
        {
            var uniqueData = UNIQUE_PREFIX + "NotRetPageWithArchivedTmpl";

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageTemplateId = await _testDataHelper.PageTemplates().AddMockTemplateAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.Publish = true;
                c.PageTemplateId = pageTemplateId;
            });
            await _testDataHelper.PageTemplates().ArchiveTemplateAsync(pageTemplateId);

            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageId })
                .AsRenderSummaries()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                pages.Should().NotBeNull();
                pages.Should().BeEmpty();
            }
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

            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageId })
                .AsRenderSummaries()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                pages.Should().NotBeNull();
                var page = pages.GetOrDefault(pageId);

                GetPageRenderSummaryByIdQueryHandlerTests.AssertBasicDataMapping(
                    addPageCommand,
                    versionId,
                    page
                    );
            }
        }
    }
}
