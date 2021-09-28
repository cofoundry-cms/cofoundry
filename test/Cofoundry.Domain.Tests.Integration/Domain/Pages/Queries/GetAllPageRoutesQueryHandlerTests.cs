using FluentAssertions;
using FluentAssertions.Execution;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetAllPageRoutesQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GAllPageRoutesQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetAllPageRoutesQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task ReturnsAllRoutes()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsAllRoutes);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var testPages = new Dictionary<int, string>();

            for (int i = 1; i < 3; i++)
            {
                var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData + i);

                for (int j = 1; j < 4; j++)
                {
                    var pageTitle = uniqueData + j;
                    var pageId = await _testDataHelper.Pages().AddAsync(pageTitle, parentDirectoryId);
                    testPages.Add(pageId, pageTitle);
                }
            }

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
