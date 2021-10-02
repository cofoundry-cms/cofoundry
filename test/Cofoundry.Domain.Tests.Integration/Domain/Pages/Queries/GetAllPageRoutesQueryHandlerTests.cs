using FluentAssertions;
using FluentAssertions.Execution;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetAllPageRoutesQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GAllPageRoutesQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetAllPageRoutesQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task ReturnsAllRoutes()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsAllRoutes);

            using var app = _appFactory.Create();
            var testPages = new Dictionary<int, string>();

            for (int i = 1; i < 3; i++)
            {
                var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData + i);

                for (int j = 1; j < 4; j++)
                {
                    var pageTitle = uniqueData + j;
                    var pageId = await app.TestData.Pages().AddAsync(pageTitle, parentDirectoryId);
                    testPages.Add(pageId, pageTitle);
                }
            }

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var results = await contentRepository
                .Pages()
                .GetAll()
                .AsRoutes()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                foreach (var testPage in testPages)
                {
                    results.Should().Contain(p => p.PageId == testPage.Key && p.Title == testPage.Value);
                }
            }
        }
    }
}
