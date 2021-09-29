using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageDetailsByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageDetailsByIdQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageDetailsByIdQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task ReturnsRequestedPage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsRequestedPage);
            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);

            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsDetails()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.PageId.Should().Be(pageId);
                page.PageRoute.Should().NotBeNull();
            }

        }

        [Fact]
        public async Task DoesNotReturnDeletedPage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(DoesNotReturnDeletedPage);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);

            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsDetails()
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

            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsDetails()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.PageId.Should().Be(pageId);

                page.AuditData.Should().NotBeNull();
                page.AuditData.Creator.Should().NotBeNull();
                page.AuditData.Creator.UserId.Should().BePositive();

                page.PageRoute.Should().NotBeNull();
                page.PageRoute.PageId.Should().Be(pageId);

                page.Tags.Should().NotBeNull();
                page.Tags.Should().HaveCount(1);
                page.Tags.Single().Should().Be(_dbDependentFixture.SeededEntities.TestTag.TagText);

                page.LatestVersion.Should().NotBeNull();
                page.LatestVersion.OpenGraph.Should().NotBeNull();
                page.LatestVersion.OpenGraph.Title.Should().Be(addPageCommand.OpenGraphTitle);
                page.LatestVersion.OpenGraph.Description.Should().Be(addPageCommand.OpenGraphDescription);
                page.LatestVersion.OpenGraph.Image.Should().NotBeNull();
                page.LatestVersion.OpenGraph.Image.ImageAssetId.Should().Be(addPageCommand.OpenGraphImageId);
                page.LatestVersion.MetaDescription.Should().Be(addPageCommand.MetaDescription);

                page.LatestVersion.AuditData.Should().NotBeNull();
                page.LatestVersion.AuditData.Creator.Should().NotBeNull();
                page.LatestVersion.AuditData.Creator.UserId.Should().BePositive();
                page.LatestVersion.DisplayVersion.Should().Be(2);
                page.LatestVersion.ShowInSiteMap.Should().Be(addPageCommand.ShowInSiteMap);
                page.LatestVersion.Title.Should().Be(addPageCommand.Title);
                page.LatestVersion.Template.Should().NotBeNull();
                page.LatestVersion.Template.PageTemplateId.Should().Be(addPageCommand.PageTemplateId);
                page.LatestVersion.PageVersionId.Should().Be(versionId);
                page.LatestVersion.WorkFlowStatus.Should().Be(WorkFlowStatus.Draft);
                page.LatestVersion.Regions.Should().NotBeNull();
                page.LatestVersion.Regions.Should().HaveCount(1);
            }
        }

        [Fact]
        public async Task WhenPublished_ReturnsLatestPublishedVersion()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenPublished_RetLatestPublishedVer";
            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().PublishAsync(pageId);
            var latestVersionId = await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().PublishAsync(pageId);

            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsDetails()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                page.LatestVersion.Should().NotBeNull();
                page.LatestVersion.WorkFlowStatus.Should().Be(WorkFlowStatus.Published);
                page.LatestVersion.PageVersionId.Should().Be(latestVersionId);
            }
        }
    }
}
