using Cofoundry.Domain.CQS;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetUpdatePageCommandByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GUpdPageCmdByIdQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetUpdatePageCommandByIdQueryHandlerTests(
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
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var addPageCommand = app.TestData.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.Tags = new string[] { UNIQUE_PREFIX + "1", app.SeededEntities.TestTag.TagText };

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
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
