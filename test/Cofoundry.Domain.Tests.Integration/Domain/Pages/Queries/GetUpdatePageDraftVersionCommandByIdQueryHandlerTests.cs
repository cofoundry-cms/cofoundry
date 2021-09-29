using Cofoundry.Domain.CQS;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class GetUpdatePageDraftVersionCommandByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GUpdPageDraftCmdByIdQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public GetUpdatePageDraftVersionCommandByIdQueryHandlerTests(
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
            addPageCommand.MetaDescription = uniqueData + " Meta";
            addPageCommand.OpenGraphDescription = uniqueData + "OG Desc";
            addPageCommand.OpenGraphImageId = _dbDependentFixture.SeededEntities.TestImageId;
            addPageCommand.OpenGraphTitle = uniqueData + "OG Title";

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var query = new GetUpdateCommandByIdQuery<UpdatePageDraftVersionCommand>(addPageCommand.OutputPageId);
            var command = await contentRepository.ExecuteQueryAsync(query);

            using (new AssertionScope())
            {
                command.Should().NotBeNull();
                command.PageId.Should().Be(addPageCommand.OutputPageId);
                command.MetaDescription.Should().Be(command.MetaDescription);
                command.OpenGraphDescription.Should().Be(command.OpenGraphDescription);
                command.OpenGraphImageId.Should().Be(command.OpenGraphImageId);
                command.OpenGraphTitle.Should().Be(command.OpenGraphTitle);
                command.Publish.Should().BeFalse();
                command.PublishDate.Should().BeNull();
                command.ShowInSiteMap.Should().Be(command.ShowInSiteMap);
                command.Title.Should().Be(command.Title);
            }
        }
    }
}
