using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.UserAreas.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetAllUserAreaMicroSummariesQueryHandlerTests
    {
        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetAllUserAreaMicroSummariesQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task ReturnsAndMaps()
        {
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var area1 = app.SeededEntities.TestUserArea1.Definition;
            var area2 = app.SeededEntities.TestUserArea2.Definition;

            var userAreas = await contentRepository
                .UserAreas()
                .GetAll()
                .AsMicroSummaries()
                .ExecuteAsync();

            using (new AssertionScope())
            {
                userAreas.Should().HaveCountGreaterOrEqualTo(2);
                userAreas.Should().BeInAscendingOrder(a => a.Name);

                var testArea1 = userAreas.SingleOrDefault(a => a.UserAreaCode == area1.UserAreaCode);
                testArea1.Should().NotBeNull();
                testArea1.Name.Should().Be(testArea1.Name);

                var testArea2 = userAreas.SingleOrDefault(a => a.UserAreaCode == area2.UserAreaCode);
                testArea2.Should().NotBeNull();
                testArea2.Name.Should().Be(testArea2.Name);

            }
        }
    }
}
