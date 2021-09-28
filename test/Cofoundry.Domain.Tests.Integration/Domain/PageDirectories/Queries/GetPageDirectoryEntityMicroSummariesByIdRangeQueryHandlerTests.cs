using Cofoundry.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetPageDirectoryEntityMicroSummariesByIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageDirEMSByIdRangeQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetPageDirectoryEntityMicroSummariesByIdRangeQueryHandlerTests(
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

            var directory1Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var directory2Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + "-sub", directory1Id);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var query = new GetPageDirectoryEntityMicroSummariesByIdRangeQuery(new int[] { directory1Id, directory2Id });
            var directoryLookup = await contentRepository.ExecuteQueryAsync(query);
            var directory1 = directoryLookup.GetOrDefault(directory1Id);
            var directory2 = directoryLookup.GetOrDefault(directory2Id);

            using (new AssertionScope())
            {
                directoryLookup.Should().NotBeNull();
                directoryLookup.Count.Should().Be(2);

                directory1.Should().NotBeNull();
                directory1.EntityDefinitionCode.Should().Be(PageDirectoryEntityDefinition.DefinitionCode);
                directory1.EntityDefinitionName.Should().Be(new PageDirectoryEntityDefinition().Name);
                directory1.IsPreviousVersion.Should().BeFalse();
                directory1.RootEntityId.Should().Be(directory1Id);
                directory1.RootEntityTitle.Should().Be(uniqueData);

                directory2.Should().NotBeNull();
            }
        }
    }
}
