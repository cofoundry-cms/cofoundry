using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class AddPageDraftVersionCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "AddPageCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public AddPageDraftVersionCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task WhenPageIsPublished_CopiesBasicData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPageIsPublished_CopiesBasicData);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.MetaDescription = uniqueData + " Meta";
                c.OpenGraphDescription = uniqueData + "OG Desc";
                c.OpenGraphImageId = app.SeededEntities.TestImageId;
                c.OpenGraphTitle = uniqueData + "OG Title";
                c.Publish = true;
            });

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var draftVersionId = await contentRepository
                .Pages()
                .Versions()
                .AddDraftAsync(new AddPageDraftVersionCommand()
                {
                    PageId = pageId
                });

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var versions = await dbContext
                .PageVersions
                .AsNoTracking()
                .Include(v => v.PageVersionBlocks)
                .FilterByPageId(pageId)
                .ToListAsync();

            var publishedVersion = versions.FirstOrDefault(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
            var draftVersion = versions.SingleOrDefault(v => v.PageVersionId == draftVersionId);

            using (new AssertionScope())
            {
                versions.Should().HaveCount(2);
                publishedVersion.Should().NotBeNull();
                draftVersion.Should().NotBeNull();
                draftVersion.CreateDate.Should().BeAfter(publishedVersion.CreateDate);
                draftVersion.DisplayVersion.Should().Be(2);
                draftVersion.ExcludeFromSitemap.Should().BeTrue();
                draftVersion.MetaDescription.Should().Be(publishedVersion.MetaDescription);
                draftVersion.OpenGraphDescription.Should().Be(publishedVersion.OpenGraphDescription);
                draftVersion.OpenGraphImageId.Should().Be(publishedVersion.OpenGraphImageId);
                draftVersion.OpenGraphTitle.Should().Be(publishedVersion.OpenGraphTitle);
                draftVersion.PageTemplateId.Should().Be(publishedVersion.PageTemplateId);
                draftVersion.PageVersionBlocks.Should().BeEmpty();
                draftVersion.Title.Should().Be(publishedVersion.Title);
                draftVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Draft);
            }
        }

        [Fact]
        public async Task WhenPageIsPublished_CopiesRegions()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPageIsPublished_CopiesRegions);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var copyFromVersionId = await dbContext
                .PageVersions
                .FilterActive()
                .FilterByPageId(pageId)
                .Select(v => v.PageVersionId)
                .SingleAsync();

            // Add some blocks to the draft
            var textBlockId = await app.TestData.Pages().AddPlainTextBlockToTestTemplateAsync(copyFromVersionId, uniqueData);
            var imageBlockId = await app.TestData.Pages().AddImageTextBlockToTestTemplateAsync(copyFromVersionId);

            // Publish the page so we can create a new draft from it
            await contentRepository
                .Pages()
                .PublishAsync(new PublishPageCommand()
                {
                    PageId = pageId
                });

            // Create the new draft
            var draftVersionId = await contentRepository
                .Pages()
                .Versions()
                .AddDraftAsync(new AddPageDraftVersionCommand()
                {
                    PageId = pageId
                });

            // Get result data to assert
            var versions = await dbContext
                .PageVersions
                .AsNoTracking()
                .Include(v => v.PageVersionBlocks)
                .FilterByPageId(pageId)
                .ToListAsync();

            var publishedVersion = versions.FirstOrDefault(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
            var publishedVersionTextBlock = publishedVersion
                .PageVersionBlocks
                .SingleOrDefault(v => v.PageVersionBlockId == textBlockId);
            var publishedVersionImageBlock = publishedVersion
                .PageVersionBlocks
                .SingleOrDefault(v => v.PageVersionBlockId == imageBlockId);

            var draftVersion = versions.SingleOrDefault(v => v.PageVersionId == draftVersionId);
            var draftVersionTextBlock = publishedVersion
                .PageVersionBlocks
                .FirstOrDefault(v => v.PageBlockTypeId == publishedVersionTextBlock?.PageBlockTypeId);
            var draftVersionImageBlock = publishedVersion
                .PageVersionBlocks
                .FirstOrDefault(v => v.PageBlockTypeId == publishedVersionImageBlock?.PageBlockTypeId);

            var unstructuredDependencies = await dbContext
                .UnstructuredDataDependencies
                .AsNoTracking()
                .Where(v => v.RootEntityDefinitionCode == PageVersionBlockEntityDefinition.DefinitionCode && v.RootEntityId == draftVersionImageBlock.PageVersionBlockId)
                .ToListAsync();

            var copiedImageDependency = unstructuredDependencies.SingleOrDefault();

            // Assert

            using (new AssertionScope())
            {
                versions.Should().HaveCount(2);
                publishedVersion.Should().NotBeNull();
                draftVersion.Should().NotBeNull();
                publishedVersion.PageVersionBlocks.Should().HaveCount(2);
                draftVersion.PageVersionBlocks.Should().HaveCount(publishedVersion.PageVersionBlocks.Count);

                AssertBlockMatches(publishedVersionTextBlock, draftVersionTextBlock);
                AssertBlockMatches(publishedVersionImageBlock, draftVersionImageBlock);
                unstructuredDependencies.Should().HaveCount(1);
                copiedImageDependency.Should().NotBeNull();
                copiedImageDependency.RelatedEntityId.Should().Be(app.SeededEntities.TestImageId);
                copiedImageDependency.RelatedEntityDefinitionCode.Should().Be(ImageAssetEntityDefinition.DefinitionCode);
            }

            static void AssertBlockMatches(PageVersionBlock publishedBlock, PageVersionBlock draftBlock)
            {
                draftBlock.Should().NotBeNull();
                draftBlock.CreateDate.Should().Be(publishedBlock.CreateDate);
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
        public async Task WhenPageHasDraft_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPageHasDraft_Throws);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var command = new AddPageDraftVersionCommand()
            {
                PageId = pageId
            };

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Awaiting(r => r.Pages().Versions().AddDraftAsync(command))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(command.PageId));
        }

        [Fact]
        public async Task WhenDraftAdded_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenDraftAdded_SendsMessage);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var draftVersionId = await contentRepository
                .Pages()
                .Versions()
                .AddDraftAsync(new AddPageDraftVersionCommand()
                {
                    PageId = pageId
                });

            app.Mocks
                .CountMessagesPublished<PageDraftVersionAddedMessage>(m => m.PageId == pageId && m.PageVersionId == draftVersionId)
                .Should().Be(1);
        }
    }
}
