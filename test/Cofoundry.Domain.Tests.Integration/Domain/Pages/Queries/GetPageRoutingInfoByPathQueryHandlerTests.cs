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
    public class GetPageRoutingInfoByPathQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageRoutingInfoByPathQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetPageRoutingInfoByPathQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Theory]
        [InlineData("https://example.com/absolutepath")]
        [InlineData("*")]
        [InlineData("/bad-pa^th/")]
        [InlineData("")]
        [InlineData(null)]
        public async Task WhenMalformedPath_ReturnsNull(string path)
        {
            using var app = _appFactory.Create();

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var result = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = path
                })
                .ExecuteAsync();

            result.Should().BeNull();
        }

        [Fact]
        public async Task WhenNotPublished_ReturnsNull()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenNotPublished_ReturnsNull);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{sluggedUniqueData}"
                })
                .ExecuteAsync();

            page.Should().BeNull();
        }

        [Fact]
        public async Task WhenUnPublished_ReturnsNull()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUnPublished_ReturnsNull);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c=> c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .UnPublishAsync(new UnPublishPageCommand(pageId));

            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{sluggedUniqueData}"
                })
                .ExecuteAsync();

            page.Should().BeNull();
        }

        [Fact]
        public async Task WhenIncludeUnpublishedTrue_ReturnsUnpublishedPage()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenIncUnpubTrue_ReturnsUnpubPage";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{sluggedUniqueData}",
                    IncludeUnpublished = true
                })
                .ExecuteAsync();

            page.Should().NotBeNull();
            page.PageRoute.PageId.Should().Be(pageId);
        }

        [Fact]
        public async Task WhenGenericRoute_Maps()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenGenericRoute_Maps);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{sluggedUniqueData}"
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.CustomEntityRoute.Should().BeNull();
                page.CustomEntityRouteRule.Should().BeNull();
                page.PageRoute.Should().NotBeNull();
                page.PageRoute.PageId.Should().Be(pageId);
            }
        }

        [Fact]
        public async Task WhenCutomEntityRoute_Maps()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenCutomEntityRoute_Maps);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var customEntityId = await app.TestData.CustomEntities().AddAsync(uniqueData, c => c.Publish = true);
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddCustomEntityPageDetailsAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{customEntityId}/{sluggedUniqueData}"
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.CustomEntityRoute.Should().NotBeNull();
                page.CustomEntityRoute.CustomEntityId.Should().Be(customEntityId);
                page.CustomEntityRouteRule.Should().NotBeNull();
                page.CustomEntityRouteRule.Should().BeOfType<IdAndUrlSlugCustomEntityRoutingRule>();
                page.PageRoute.Should().NotBeNull();
                page.PageRoute.PageId.Should().Be(pageId);
            }
        }

        [Fact]
        public async Task WhenIncludeUnpublishedTrue_ReturnsUnpublishedCustomEntityPage()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenIncUnpubTrue_ReturnsCEUnpubPage";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var customEntityId = await app.TestData.CustomEntities().AddAsync(uniqueData);
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddCustomEntityPageDetailsAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{customEntityId}/{sluggedUniqueData}",
                    IncludeUnpublished = true
                })
                .ExecuteAsync();

            page.Should().NotBeNull();
            page.PageRoute.PageId.Should().Be(pageId);
            page.CustomEntityRoute.Should().NotBeNull();
            page.CustomEntityRoute.CustomEntityId.Should().Be(customEntityId);
        }

        [Fact]
        public async Task WhenCustomEntityNotPublished_ReturnsNull()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenCENotPub_RetNull";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var customEntityId = await app.TestData.CustomEntities().AddAsync(uniqueData);
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            await app.TestData.Pages().AddCustomEntityPageDetailsAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{customEntityId}/{sluggedUniqueData}"
                })
                .ExecuteAsync();

            page.Should().BeNull();
        }

        [Fact]
        public async Task WhenCustomEntityUnPublished_ReturnsNull()
        {
            var uniqueData = UNIQUE_PREFIX + "WhenCEUnPublished_ReturnsNull";
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var customEntityId = await app.TestData.CustomEntities().AddAsync(uniqueData);
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .CustomEntities()
                .UnPublishAsync(new UnPublishCustomEntityCommand(customEntityId));

            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{customEntityId}/{sluggedUniqueData}"
                })
                .ExecuteAsync();

            page.Should().BeNull();
        }

        [Fact]
        public async Task DoesNotReturnDeletedRoute()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(DoesNotReturnDeletedRoute);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            var page = await contentRepository
                .Pages()
                .GetByPath()
                .AsRoutingInfo(new GetPageRoutingInfoByPathQuery()
                {
                    Path = $"/{sluggedUniqueData}/{sluggedUniqueData}"
                })
                .ExecuteAsync();

            page.Should().BeNull();
        }
    }
}
