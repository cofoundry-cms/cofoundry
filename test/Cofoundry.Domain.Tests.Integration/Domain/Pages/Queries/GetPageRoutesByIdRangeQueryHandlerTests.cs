using Cofoundry.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetPageRoutesByIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GAllPageRoutesQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetPageRoutesByIdRangeQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task ReturnsRequestedRoutes()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsRequestedRoutes);

            using var app = _appFactory.Create();
            var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");
            var page1Id = await app.TestData.Pages().AddAsync(uniqueData + "1", directory1Id);
            var page2Id = await app.TestData.Pages().AddAsync(uniqueData + "2", directory1Id);
            var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "2");
            var page3Id = await app.TestData.Pages().AddAsync(uniqueData + "3", directory2Id);
            var page4Id = await app.TestData.Pages().AddAsync(uniqueData + "4", directory2Id);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var results = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { page1Id, page3Id, page4Id })
                .AsRoutes()
                .ExecuteAsync();

            var page1 = results.GetOrDefault(page1Id);
            var page2 = results.GetOrDefault(page2Id);
            var page3 = results.GetOrDefault(page3Id);
            var page4 = results.GetOrDefault(page4Id);

            using (new AssertionScope())
            {
                page1.Should().NotBeNull();
                page1.PageId.Should().Be(page1Id);

                page3.Should().NotBeNull();
                page3.PageId.Should().Be(page3Id);

                page4.Should().NotBeNull();
                page4.PageId.Should().Be(page4Id);
            }
        }

        [Fact]
        public async Task DoesNotReturnDeletedRoutes()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(DoesNotReturnDeletedRoutes);

            using var app = _appFactory.Create();
            var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "1");
            var page1Id = await app.TestData.Pages().AddAsync(uniqueData + "1", directory1Id);
            var page2Id = await app.TestData.Pages().AddAsync(uniqueData + "2", directory1Id);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .DeleteAsync(page1Id);
            await contentRepository
                .Pages()
                .DeleteAsync(page2Id);

            var results = await contentRepository
                .Pages()
                .GetByIdRange(new int[] { page1Id, page2Id })
                .AsRoutes()
                .ExecuteAsync();

            results.Should().HaveCount(0);
        }
    }
}
