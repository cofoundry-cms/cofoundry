using Cofoundry.Core;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetPageRouteByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageRouteByIdQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetPageRouteByIdQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task ReturnsRequestedRoute()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsRequestedRoute);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var rootDirectoryId = await app.TestData.PageDirectories().GetRootDirectoryIdAsync();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var page1Id = await app.TestData.Pages().AddAsync(uniqueData + "1", directoryId);
            var page2Id = await app.TestData.Pages().AddAsync(uniqueData + "2", directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page2 = await contentRepository
                .Pages()
                .GetById(page2Id)
                .AsRoute()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                page2.Should().NotBeNull();
                page2.PageId.Should().Be(page2Id);

                page2.FullUrlPath.Should().Be($"/{sluggedUniqueData}/{sluggedUniqueData}2");
                page2.HasDraftVersion.Should().BeTrue();
                page2.HasPublishedVersion.Should().BeFalse();
                page2.Locale.Should().BeNull();
                page2.PageId.Should().Be(page2Id);
                page2.PageType.Should().Be(PageType.Generic);
                page2.PublishStatus.Should().Be(PublishStatus.Unpublished);
                page2.Title.Should().Be(uniqueData + "2");
                page2.UrlPath.Should().Be(sluggedUniqueData + "2");
                page2.Versions.Should().HaveCount(1);

                page2.PageDirectory.Should().NotBeNull();
                page2.PageDirectory.PageDirectoryId.Should().Be(directoryId);
                page2.PageDirectory.FullUrlPath.Should().Be($"/{sluggedUniqueData}");
                page2.PageDirectory.LocaleVariations.Should().BeEmpty();
                page2.PageDirectory.Name.Should().Be(uniqueData);
                page2.PageDirectory.ParentPageDirectoryId.Should().Be(rootDirectoryId);
                page2.PageDirectory.UrlPath.Should().Be(sluggedUniqueData);

                var version = page2.Versions.FirstOrDefault();
                version.CreateDate.Should().NotBeDefault();
                version.HasCustomEntityRegions.Should().BeFalse();
                version.HasPageRegions.Should().BeTrue();
                version.IsLatestPublishedVersion.Should().BeFalse();
                version.PageTemplateId.Should().Be(app.SeededEntities.TestPageTemplate.PageTemplateId);
                version.Title.Should().Be(uniqueData + "2");
                version.VersionId.Should().BePositive();
                version.WorkFlowStatus.Should().Be(WorkFlowStatus.Draft);
            }

        }

        [Fact]
        public async Task DoesNotReturnDeletedRoutes()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(DoesNotReturnDeletedRoutes);

            using var app = _appFactory.Create();
            var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directory1Id);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            var page = await contentRepository
                .Pages()
                .GetById(pageId)
                .AsRoute()
                .ExecuteAsync();

            page.Should().BeNull();
        }
    }
}
