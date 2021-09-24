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
    [Collection(nameof(DbDependentFixture))]
    public class DuplicatePageCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DuplicatePageCHT ";
        private readonly TestDataHelper _testDataHelper;

        private readonly DbDependentFixture _dbDependentFixture;

        public DuplicatePageCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task CopiesBasicData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CopiesBasicData);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var newDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData + " Copy");
            var originalPageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.MetaDescription = uniqueData + " Meta";
                c.OpenGraphDescription = uniqueData + "OG Desc";
                c.OpenGraphImageId = _dbDependentFixture.SeededEntities.TestImageId;
                c.OpenGraphTitle = uniqueData + "OG Title";
                c.Publish = true;
            });

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var command = new DuplicatePageCommand()
            {
                PageToDuplicateId = originalPageId,
                PageDirectoryId = newDirectoryId,
                Title = uniqueData + " Page Copy",
                UrlPath = "page-copy"
            };

            var newPageId = await contentRepository
                .Pages()
                .DuplicateAsync(command);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

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
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var originalPageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            var pageVersionId = await _testDataHelper.Pages().AddDraftAsync(originalPageId);
            var textBlockId = await _testDataHelper.Pages().AddPlainTextBlockToTestTemplateAsync(pageVersionId, uniqueData);
            var imageBlockId = await _testDataHelper.Pages().AddImageTextBlockToTestTemplateAsync(pageVersionId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var command = new DuplicatePageCommand()
            {
                PageToDuplicateId = originalPageId,
                PageDirectoryId = directoryId,
                Title = uniqueData + " Page Copy",
                UrlPath = "page-copy"
            };

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
                copiedImageDependency.RelatedEntityId.Should().Be(_dbDependentFixture.SeededEntities.TestImageId);
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
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.Publish = true;
                c.MetaDescription = "Test Meta";
                c.OpenGraphTitle = "Test OG Title";
                c.ShowInSiteMap = true;
            });

            var version2Id = await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().AddPlainTextBlockToTestTemplateAsync(version2Id, uniqueData);
            await _testDataHelper.Pages().AddImageTextBlockToTestTemplateAsync(version2Id);
            await _testDataHelper.Pages().PublishAsync(pageId);
            var version3Id = await _testDataHelper.Pages().AddDraftAsync(pageId);


            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            await contentRepository
                .Pages()
                .Versions()
                .UpdateDraftAsync(new UpdatePageDraftVersionCommand()
                {
                    PageId = pageId,
                    Title = uniqueData + "Updated Title"
                });

            await _testDataHelper.Pages().AddPlainTextBlockToTestTemplateAsync(version3Id, uniqueData + " V3");
            await _testDataHelper.Pages().PublishAsync(pageId);

            await contentRepository
                .Pages()
                .Versions()
                .AddDraftAsync(new AddPageDraftVersionCommand()
                {
                    PageId = pageId,
                    CopyFromPageVersionId = version2Id
                });

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
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var newPageId = await contentRepository
                .Pages()
                .DuplicateAsync(new DuplicatePageCommand()
                {
                    PageDirectoryId = directoryId,
                    PageToDuplicateId = pageId,
                    Title = uniqueData + "Copy",
                    UrlPath = "copy"
                });

            var draftVersionId = await dbContext
                .PageVersions
                .FilterByPageId(newPageId)
                .Select(p => p.PageVersionId)
                .SingleAsync();

            scope
                .CountMessagesPublished<PageAddedMessage>(m => m.PageId == newPageId && !m.HasPublishedVersionChanged)
                .Should().Be(1);
        }

        private async Task<Page> GetPageFromDb(int pageId)
        {
            using var scope = _dbDependentFixture.CreateServiceScope();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            return await dbContext
                .Pages
                .AsNoTracking()
                .Include(v => v.PageVersions)
                .ThenInclude(v => v.PageVersionBlocks)
                .FilterByPageId(pageId)
                .SingleOrDefaultAsync();
        }
    }
}
