using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetUpdatePageDirectoryCommandByIdQueryHandlerTests
    {
        const string DIRECTORY_PREFIX = "GUpdPageDirCmdByIdCHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetUpdatePageDirectoryCommandByIdQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task ReturnsMappedData()
        {
            var uniqueData = DIRECTORY_PREFIX + nameof(ReturnsMappedData);

            var parentDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var addDirectoryCommand = _testDataHelper.PageDirectories().CreateAddCommand(uniqueData, parentDirectoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var query = new GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand>(addDirectoryCommand.OutputPageDirectoryId);
            var command = await contentRepository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                command.Should().NotBeNull();
                command.Name.Should().Be(addDirectoryCommand.Name);
                command.PageDirectoryId.Should().Be(addDirectoryCommand.OutputPageDirectoryId);
                command.ParentPageDirectoryId.Should().Be(parentDirectoryId);
                command.UrlPath.Should().Be(command.UrlPath);
            }
        }

        [Fact]
        public async Task WhenRootDirectory_Throws()
        {
            var rootDirectoryId = await _testDataHelper.PageDirectories().GetRootDirectoryIdAsync();
            var query = new GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand>(rootDirectoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .Awaiting(r => r.ExecuteQueryAsync(query))
                .Should()
                .ThrowAsync<NotPermittedException>();
        }
    }
}
