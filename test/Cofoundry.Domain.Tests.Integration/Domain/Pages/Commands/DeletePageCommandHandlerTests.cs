using Cofoundry.Domain.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands
{
    [Collection(nameof(DbDependentFixture))]
    public class DeletePageCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DelPageCHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public DeletePageCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task CanDelete()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanDelete);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.Publish = true;
                c.OpenGraphImageId = _dbDependentFixture.SeededEntities.TestImageId;
                c.Tags.Add(_dbDependentFixture.SeededEntities.TestTag.TagText);
            });
            var draftVersionId = await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().AddPlainTextBlockToTestTemplateAsync(draftVersionId);
            await _testDataHelper.Pages().AddImageTextBlockToTestTemplateAsync(draftVersionId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            var page = await dbContext
                .Pages
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.PageId == pageId);

            page.Should().BeNull();
        }

        [Fact]
        public async Task WhenDeleted_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenDeleted_SendsMessage);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            scope
                .CountMessagesPublished<PageDeletedMessage>(m => m.PageId == pageId)
                .Should().Be(1);
        }
    }
}
