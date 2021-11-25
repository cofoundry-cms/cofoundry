using Cofoundry.Core;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetPageSummariesByIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageSumByIdRangeQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetPageSummariesByIdRangeQueryHandlerTests(
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

            var pages = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { pageId })
                .AsSummaries()
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
                .AsSummaries()
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
                .AsSummaries()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                pages.Should().NotBeNull();
                var page = pages.GetOrDefault(pageId);

                AssertBasicDataMapping(
                    uniqueData,
                    addPageCommand,
                    page
                    );
            }
        }

        internal static void AssertBasicDataMapping(
            string uniqueData,
            AddPageCommand addPageCommand,
            PageSummary page
            )
        {
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            page.Should().NotBeNull();
            page.PageId.Should().Be(addPageCommand.OutputPageId);

            page.AuditData.Should().NotBeNull();
            page.AuditData.Creator.Should().NotBeNull();
            page.AuditData.Creator.UserId.Should().BePositive();

            page.FullUrlPath.Should().Be($"/{sluggedUniqueData}/{addPageCommand.UrlPath}");

            page.Should().NotBeNull();
            page.HasDraftVersion.Should().BeFalse();
            page.Locale.Should().BeNull();
            page.PageType.Should().Be(addPageCommand.PageType);
            page.PublishDate.Should().NotBeNull().And.NotBeDefault();
            page.LastPublishDate.Should().NotBeNull().And.NotBeDefault();
            page.PublishStatus.Should().Be(PublishStatus.Published);
            page.Tags.Should().ContainSingle(t => t == "Test");
            page.Title.Should().Be(addPageCommand.Title);
            page.UrlPath.Should().Be(addPageCommand.UrlPath);
        }

    }
}
