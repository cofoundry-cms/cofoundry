using Cofoundry.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetPageDirectoryEntityMicroSummariesByIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GPageDirEMSByIdRangeQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetPageDirectoryEntityMicroSummariesByIdRangeQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task ReturnsMappedData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(ReturnsMappedData);

            using var app = _appFactory.Create();
            var directory1Id = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var directory2Id = await app.TestData.PageDirectories().AddAsync(uniqueData + "-sub", directory1Id);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
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
