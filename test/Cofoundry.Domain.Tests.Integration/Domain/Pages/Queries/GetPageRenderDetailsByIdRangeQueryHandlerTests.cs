using Cofoundry.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetPageRenderDetailsByIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageRenderDetailsByIdRangeQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetPageRenderDetailsByIdRangeQueryHandlerTests(
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
                .GetByIdRange(new int[] { pageWithDraftOnlyId, pageWithPublishedOnlyId, pageWithPublishedandDraftId })
                .AsRenderDetails(publishStatus)
                .ExecuteAsync();

            AssertStatus(pages, pageWithDraftOnlyId, draftOnlyWorkFlowStatus);
            AssertStatus(pages, pageWithPublishedOnlyId, publishOnlyWorkFlowStatus);
            AssertStatus(pages, pageWithPublishedandDraftId, publishAndDraftWorkFlowStatus);

            static void AssertStatus(
                IDictionary<int, PageRenderDetails> pages,
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
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .Awaiting(r => r
                    .Pages()
                    .GetByIdRange(new int[] { 1 })
                    .AsRenderDetails(PublishStatusQuery.SpecificVersion)
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

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageId })
                .AsRenderDetails()
                .ExecuteAsync();

            pages.Should().NotBeNull();
            pages.Should().BeEmpty();
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
            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageId })
                .AsRenderDetails()
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

            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageId })
                .AsRenderDetails()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                pages.Should().NotBeNull();
                var page = pages.GetOrDefault(pageId);

                GetPageRenderDetailsByIdQueryHandlerTests.AssertBasicDataMapping(
                    addPageCommand,
                    versionId,
                    page
                    );
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
            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageId })
                .AsRenderDetails()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                pages.Should().NotBeNull();
                var page = pages.GetOrDefault(pageId);

                GetPageRenderDetailsByIdQueryHandlerTests.AssertBlockDataMapping(
                    uniqueData,
                    plainTextBlockId,
                    imageBlockId,
                    page
                    );
            }
        }
    }
}
