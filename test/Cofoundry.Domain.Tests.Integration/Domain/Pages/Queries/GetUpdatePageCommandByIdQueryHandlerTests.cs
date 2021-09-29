using Cofoundry.Domain.CQS;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetUpdatePageCommandByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GUpdPageCmdByIdQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetUpdatePageCommandByIdQueryHandlerTests(
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
            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.Tags = new string[] { UNIQUE_PREFIX + "1", _dbDependentFixture.SeededEntities.TestTag.TagText };

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var query = new GetUpdateCommandByIdQuery<UpdatePageCommand>(addPageCommand.OutputPageId);
            var command = await contentRepository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                command.Should().NotBeNull();
                command.PageId.Should().Be(addPageCommand.OutputPageId);
                command.Tags.Should().OnlyContain(t => addPageCommand.Tags.Contains(t));
            }
        }
    }
}
