using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.PageDirectories.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetUpdatePageDirectoryCommandByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GUpdPageDirCmdByIdQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetUpdatePageDirectoryCommandByIdQueryHandlerTests(
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
            var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var addDirectoryCommand = app.TestData.PageDirectories().CreateAddCommand(uniqueData, parentDirectoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .PageDirectories()
                .AddAsync(addDirectoryCommand);

            var query = new GetPatchableCommandByIdQuery<UpdatePageDirectoryCommand>(addDirectoryCommand.OutputPageDirectoryId);
            var command = await contentRepository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                command.Should().NotBeNull();
                command.Name.Should().Be(addDirectoryCommand.Name);
                command.PageDirectoryId.Should().Be(addDirectoryCommand.OutputPageDirectoryId);
            }
        }

        [Fact]
        public async Task WhenRootDirectory_Throws()
        {
            using var app = _appFactory.Create();
            var rootDirectoryId = await app.TestData.PageDirectories().GetRootDirectoryIdAsync();
            var query = new GetPatchableCommandByIdQuery<UpdatePageDirectoryCommand>(rootDirectoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Awaiting(r => r.ExecuteQueryAsync(query))
                .Should()
                .ThrowAsync<NotPermittedException>();
        }
    }
}
