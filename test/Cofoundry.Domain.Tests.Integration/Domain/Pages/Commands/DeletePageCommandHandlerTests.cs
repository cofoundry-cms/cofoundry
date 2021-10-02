using Cofoundry.Domain.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class DeletePageCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "DelPageCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public DeletePageCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanDelete()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanDelete);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.Publish = true;
                c.OpenGraphImageId = app.SeededEntities.TestImageId;
                c.Tags.Add(app.SeededEntities.TestTag.TagText);
            });
            var draftVersionId = await app.TestData.Pages().AddDraftAsync(pageId);
            await app.TestData.Pages().AddPlainTextBlockToTestTemplateAsync(draftVersionId);
            await app.TestData.Pages().AddImageTextBlockToTestTemplateAsync(draftVersionId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
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

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            app.Mocks
                .CountMessagesPublished<PageDeletedMessage>(m => m.PageId == pageId)
                .Should().Be(1);
        }
    }
}
