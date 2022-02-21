using Cofoundry.Domain.CQS;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetUpdatePageDraftVersionCommandByIdQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GUpdPageDraftCmdByIdQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetUpdatePageDraftVersionCommandByIdQueryHandlerTests(
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
            addPageCommand.MetaDescription = uniqueData + " Meta";
            addPageCommand.OpenGraphDescription = uniqueData + "OG Desc";
            addPageCommand.OpenGraphImageId = app.SeededEntities.TestImageId;
            addPageCommand.OpenGraphTitle = uniqueData + "OG Title";

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var query = new GetPatchableCommandByIdQuery<UpdatePageDraftVersionCommand>(addPageCommand.OutputPageId);
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
