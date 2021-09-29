using Cofoundry.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageEntityMicroSummariesByIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageEMSByIdRangeQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageEntityMicroSummariesByIdRangeQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task ReturnsMappedData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsMappedData);

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var page1Id = await _testDataHelper.Pages().AddAsync(uniqueData + "1", directoryId);
            var page2Id = await _testDataHelper.Pages().AddAsync(uniqueData + "2", directoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var query = new GetPageEntityMicroSummariesByIdRangeQuery(new int[] { page1Id, page2Id });
            var pageLookup = await contentRepository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                pageLookup.Should().NotBeNull();
                pageLookup.Count.Should().Be(2);

                var page1 = pageLookup.GetOrDefault(page1Id);
                page1.Should().NotBeNull();
                page1.EntityDefinitionCode.Should().Be(PageEntityDefinition.DefinitionCode);
                page1.EntityDefinitionName.Should().Be(new PageEntityDefinition().Name);
                page1.IsPreviousVersion.Should().BeFalse();
                page1.RootEntityId.Should().Be(page1Id);
                page1.RootEntityTitle.Should().Be(uniqueData + "1");

                var page2 = pageLookup.GetOrDefault(page2Id);
                page2.Should().NotBeNull();
            }
        }
    }
}
