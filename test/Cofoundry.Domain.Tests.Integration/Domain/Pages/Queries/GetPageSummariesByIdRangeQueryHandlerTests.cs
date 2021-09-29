using Cofoundry.Core;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageSummariesByIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageSumByIdRangeQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageSummariesByIdRangeQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
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
                .AsSummaries()
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

            page.FullPath.Should().Be($"/{sluggedUniqueData}/{addPageCommand.UrlPath}");

            page.Should().NotBeNull();
            page.HasDraftVersion.Should().BeFalse();
            page.Locale.Should().BeNull();
            page.PageType.Should().Be(addPageCommand.PageType);
            page.PublishDate.Should().NotBeNull().And.NotBeDefault();
            page.PublishStatus.Should().Be(PublishStatus.Published);
            page.Tags.Should().ContainSingle(t => t == "Test");
            page.Title.Should().Be(addPageCommand.Title);
            page.UrlPath.Should().Be(addPageCommand.UrlPath);
        }

    }
}
