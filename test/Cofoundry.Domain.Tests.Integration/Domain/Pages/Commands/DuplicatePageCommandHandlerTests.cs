using Cofoundry.Domain.Data;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands
{
    /// <remarks>
    /// Note that this handler calls AddPageCommandHandler internally
    /// so there is no need to repeat tests for Url uniqueness etc.
    /// </remarks>
    [Collection(nameof(DbDependentFixtureCollection))]
    public class DuplicatePageCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DuplicatePageCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public DuplicatePageCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CopiesBasicData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CopiesBasicData);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var newDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData + " Copy");
            var originalPageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.MetaDescription = uniqueData + " Meta";
                c.OpenGraphDescription = uniqueData + "OG Desc";
                c.OpenGraphImageId = app.SeededEntities.TestImageId;
                c.OpenGraphTitle = uniqueData + "OG Title";
                c.Publish = true;
            });

            var command = new DuplicatePageCommand()
            {
                PageToDuplicateId = originalPageId,
                PageDirectoryId = newDirectoryId,
                Title = uniqueData + " Page Copy",
                UrlPath = "page-copy"
            };

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var newPageId = await contentRepository
                .Pages()
                .DuplicateAsync(command);

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var originalPage = await GetPageFromDb(originalPageId);
            var originalPageVersion = originalPage?.PageVersions.FirstOrDefault();
            var newPage = await GetPageFromDb(newPageId);
            var newPageVersion = newPage?.PageVersions.FirstOrDefault();

            using (new AssertionScope())
            {
                originalPage.Should().NotBeNull();

                newPage.Should().NotBeNull();
                newPage.LocaleId.Should().Be(command.LocaleId);
                newPage.PageDirectoryId.Should().Be(command.PageDirectoryId);
                newPage.PageTypeId.Should().Be(originalPage.PageTypeId);
                newPage.PageVersions.Should().HaveCount(1);
                newPage.PublishDate.Should().BeNull();
                newPage.LastPublishDate.Should().BeNull();
                newPage.PublishStatusCode.Should().Be(PublishStatusCode.Unpublished);
                newPage.UrlPath.Should().Be(command.UrlPath);

                newPageVersion.Should().NotBeNull();
                newPageVersion.CreateDate.Should().BeAfter(originalPageVersion.CreateDate);
                newPageVersion.DisplayVersion.Should().Be(1);
                newPageVersion.ExcludeFromSitemap.Should().Be(originalPageVersion.ExcludeFromSitemap);
                newPageVersion.MetaDescription.Should().Be(originalPageVersion.MetaDescription);
                newPageVersion.OpenGraphDescription.Should().Be(originalPageVersion.OpenGraphDescription);
                newPageVersion.OpenGraphImageId.Should().Be(originalPageVersion.OpenGraphImageId);
                newPageVersion.OpenGraphTitle.Should().Be(originalPageVersion.OpenGraphTitle);
                newPageVersion.PageTemplateId.Should().Be(originalPageVersion.PageTemplateId);
                newPageVersion.PageVersionBlocks.Should().BeEmpty();
                newPageVersion.Title.Should().Be(command.Title);
                newPageVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Draft);
            }
        }

        [Fact]
        public async Task CopiesRegions()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CopiesRegions);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var originalPageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            var pageVersionId = await app.TestData.Pages().AddDraftAsync(originalPageId);
            var textBlockId = await app.TestData.Pages().AddPlainTextBlockToTestTemplateAsync(pageVersionId, uniqueData);
            var imageBlockId = await app.TestData.Pages().AddImageTextBlockToTestTemplateAsync(pageVersionId);

            var command = new DuplicatePageCommand()
            {
                PageToDuplicateId = originalPageId,
                PageDirectoryId = directoryId,
                Title = uniqueData + " Page Copy",
                UrlPath = "page-copy"
            };

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var newPageId = await contentRepository
                .Pages()
                .DuplicateAsync(command);

            // Prepare assertion data

            var originalPage = await GetPageFromDb(originalPageId);
            var originalPageVersion = originalPage?
                .PageVersions
                .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .FirstOrDefault();
            var newPage = await GetPageFromDb(newPageId);
            var newPageVersion = newPage?.PageVersions.FirstOrDefault();

            var originalVersionTextBlock = originalPageVersion?
                .PageVersionBlocks
                .SingleOrDefault(v => v.PageVersionBlockId == textBlockId);
            var originalVersionImageBlock = originalPageVersion?
                .PageVersionBlocks
                .SingleOrDefault(v => v.PageVersionBlockId == imageBlockId);

            var newVersionTextBlock = newPageVersion
                .PageVersionBlocks
                .FirstOrDefault(v => v.PageBlockTypeId == originalVersionTextBlock?.PageBlockTypeId);
            var newVersionImageBlock = newPageVersion
                .PageVersionBlocks
                .FirstOrDefault(v => v.PageBlockTypeId == originalVersionImageBlock?.PageBlockTypeId);

            var newVersionImageBlockId = newVersionImageBlock?.PageVersionBlockId;

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var unstructuredDependencies = await dbContext
                .UnstructuredDataDependencies
                .AsNoTracking()
                .Where(v => v.RootEntityDefinitionCode == PageVersionBlockEntityDefinition.DefinitionCode && v.RootEntityId == newVersionImageBlockId)
                .ToListAsync();

            var copiedImageDependency = unstructuredDependencies.SingleOrDefault();

            // Assert

            using (new AssertionScope())
            {
                originalPageVersion.Should().NotBeNull();
                newPageVersion.Should().NotBeNull();
                newPageVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Draft);
                newPageVersion.PageVersionBlocks.Should().HaveCount(originalPageVersion.PageVersionBlocks.Count);

                AssertBlockMatches(originalVersionTextBlock, newVersionTextBlock);
                AssertBlockMatches(originalVersionImageBlock, newVersionImageBlock);
                unstructuredDependencies.Should().HaveCount(1);
                copiedImageDependency.Should().NotBeNull();
                copiedImageDependency.RelatedEntityId.Should().Be(app.SeededEntities.TestImageId);
                copiedImageDependency.RelatedEntityDefinitionCode.Should().Be(ImageAssetEntityDefinition.DefinitionCode);
            }

            static void AssertBlockMatches(PageVersionBlock publishedBlock, PageVersionBlock draftBlock)
            {
                draftBlock.Should().NotBeNull();
                draftBlock.CreateDate.Should().BeAfter(publishedBlock.CreateDate);
                draftBlock.Ordering.Should().Be(publishedBlock.Ordering);
                draftBlock.PageBlockTypeId.Should().Be(publishedBlock.PageBlockTypeId);
                draftBlock.PageBlockTypeTemplateId.Should().Be(publishedBlock.PageBlockTypeTemplateId);
                draftBlock.PageTemplateRegionId.Should().Be(publishedBlock.PageTemplateRegionId);
                draftBlock.SerializedData.Should().Be(publishedBlock.SerializedData);
                draftBlock.UpdateDate.Should().Be(draftBlock.CreateDate);
            }
        }

        [Fact]
        public async Task CanCopyFromPreviousVersion()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanCopyFromPreviousVersion);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.Publish = true;
                c.MetaDescription = "Test Meta";
                c.OpenGraphTitle = "Test OG Title";
                c.ShowInSiteMap = true;
            });

            var version2Id = await app.TestData.Pages().AddDraftAsync(pageId);
            await app.TestData.Pages().AddPlainTextBlockToTestTemplateAsync(version2Id, uniqueData);
            await app.TestData.Pages().AddImageTextBlockToTestTemplateAsync(version2Id);
            await app.TestData.Pages().PublishAsync(pageId);
            var version3Id = await app.TestData.Pages().AddDraftAsync(pageId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .Versions()
                .UpdateDraftAsync(new UpdatePageDraftVersionCommand()
                {
                    PageId = pageId,
                    Title = uniqueData + "Updated Title"
                });

            await app.TestData.Pages().AddPlainTextBlockToTestTemplateAsync(version3Id, uniqueData + " V3");
            await app.TestData.Pages().PublishAsync(pageId);

            await contentRepository
                .Pages()
                .Versions()
                .AddDraftAsync(new AddPageDraftVersionCommand()
                {
                    PageId = pageId,
                    CopyFromPageVersionId = version2Id
                });

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var versions = await dbContext
                .PageVersions
                .AsNoTracking()
                .Include(v => v.PageVersionBlocks)
                .FilterActive()
                .FilterByPageId(pageId)
                .ToListAsync();

            var version2 = versions.SingleOrDefault(v => v.PageVersionId == version2Id);
            var version4 = versions.Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft).SingleOrDefault();

            using (new AssertionScope())
            {
                version2.Should().NotBeNull();
                version4.Should().NotBeNull();
                version4.ExcludeFromSitemap.Should().Be(version2.ExcludeFromSitemap);
                version4.MetaDescription.Should().Be(version2.MetaDescription);
                version4.OpenGraphDescription.Should().Be(version2.OpenGraphDescription);
                version4.OpenGraphImageId.Should().Be(version2.OpenGraphImageId);
                version4.OpenGraphTitle.Should().Be(version2.OpenGraphTitle);
                version4.PageTemplateId.Should().Be(version2.PageTemplateId);
                version4.PageVersionBlocks.Should().HaveCount(version2.PageVersionBlocks.Count());
                version4.Title.Should().Be(version2.Title);
            }
        }

        [Fact]
        public async Task WhenCopied_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenCopied_SendsMessage);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var newPageId = await contentRepository
                .Pages()
                .DuplicateAsync(new DuplicatePageCommand()
                {
                    PageDirectoryId = directoryId,
                    PageToDuplicateId = pageId,
                    Title = uniqueData + "Copy",
                    UrlPath = "copy"
                });

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var draftVersionId = await dbContext
                .PageVersions
                .FilterByPageId(newPageId)
                .Select(p => p.PageVersionId)
                .SingleAsync();

            app.Mocks
                .CountMessagesPublished<PageAddedMessage>(m => m.PageId == newPageId && !m.HasPublishedVersionChanged)
                .Should().Be(1);
        }

        private async Task<Page> GetPageFromDb(int pageId)
        {
            using var app = _appFactory.Create();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            return await dbContext
                .Pages
                .AsNoTracking()
                .Include(v => v.PageVersions)
                .ThenInclude(v => v.PageVersionBlocks)
                .FilterById(pageId)
                .SingleOrDefaultAsync();
        }
    }
}
