using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class UpdatePageUrlCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "UpdPageUrlCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public UpdatePageUrlCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanChangeDirectory()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanChangeDirectory);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var newDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData + " Copy");
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId);

            var command = new UpdatePageUrlCommand()
            {
                PageId = pageId,
                PageDirectoryId = newDirectoryId,
                UrlPath = "moved-to-new-directory"
            };

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Pages()
                .UpdateUrlAsync(command);

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var page = await dbContext
                .Pages
                .AsNoTracking()
                .FilterActive()
                .FilterById(pageId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.LocaleId.Should().Be(command.LocaleId);
                page.PageDirectoryId.Should().Be(command.PageDirectoryId);
                page.UrlPath.Should().Be(command.UrlPath);
            }
        }

        [Fact]
        public async Task CanChangeCustomEntityPageUrl()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanChangeCustomEntityPageUrl);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var addPageCommand = app.TestData.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var pageId = await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var routingRuleFormat = new IdCustomEntityRoutingRule().RouteFormat;
            var command = new UpdatePageUrlCommand()
            {
                PageId = pageId,
                PageDirectoryId = directoryId,
                CustomEntityRoutingRule = routingRuleFormat
            };

            await contentRepository
                .Pages()
                .UpdateUrlAsync(command);

            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var page = await dbContext
                .Pages
                .AsNoTracking()
                .FilterActive()
                .FilterById(pageId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.LocaleId.Should().Be(command.LocaleId);
                page.PageDirectoryId.Should().Be(command.PageDirectoryId);
                page.UrlPath.Should().Be(command.CustomEntityRoutingRule);
            }
        }

        [Fact]
        public async Task WhenRoutingRuleNotExists_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenRoutingRuleNotExists_Throws);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var addPageCommand = app.TestData.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var pageId = await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var command = new UpdatePageUrlCommand()
            {
                PageId = pageId,
                PageDirectoryId = directoryId,
                CustomEntityRoutingRule = "{not-a-routing-rule}"
            };

            await contentRepository
                .Awaiting(r => r.Pages().UpdateUrlAsync(command))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(command.CustomEntityRoutingRule));
        }

        [Fact]
        public async Task WhenRoutingRuleUniquenessRequirementNotSupported_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "RuleUniquenessReqNotSupported";

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var addPage1Command = app.TestData.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);
            var addPage2Command = app.TestData.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);
            addPage2Command.CustomEntityRoutingRule = new IdCustomEntityRoutingRule().RouteFormat;

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var page1Id = await contentRepository
                .Pages()
                .AddAsync(addPage1Command);

            var page2Id = await contentRepository
                .Pages()
                .AddAsync(addPage2Command);

            var command = new UpdatePageUrlCommand()
            {
                PageId = page1Id,
                PageDirectoryId = directoryId,
                CustomEntityRoutingRule = new IdCustomEntityRoutingRule().RouteFormat
            };

            await contentRepository
                .Awaiting(r => r.Pages().UpdateUrlAsync(command))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(command.CustomEntityRoutingRule));
        }

        [Fact]
        public async Task WhenUrlNotUnique_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUrlNotUnique_Throws);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var page1Id = await app.TestData.Pages().AddAsync(uniqueData, directoryId);
            var page2Id = await app.TestData.Pages().AddAsync(uniqueData + "2", directoryId);

            var command = new UpdatePageUrlCommand()
            {
                PageId = page2Id,
                PageDirectoryId = directoryId,
                UrlPath = SlugFormatter.ToSlug(uniqueData)
            };

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            await contentRepository
                .Awaiting(r => r.Pages().UpdateUrlAsync(command))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException>()
                .WithMemberNames(nameof(command.UrlPath));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WhenUrlUpdated_SendsCorrectMessage(bool isPublished)
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenUrlUpdated_SendsCorrectMessage) + isPublished.ToString()[0];
            var uniqueDataSlug = SlugFormatter.ToSlug(uniqueData);

            using var app = _appFactory.Create();
            var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
            var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = isPublished);

            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            await contentRepository
                .Pages()
                .UpdateUrlAsync(new UpdatePageUrlCommand()
                {
                    PageId = pageId,
                    PageDirectoryId = directoryId,
                    UrlPath = "copy"
                });

            app.Mocks
                .CountMessagesPublished<PageUrlChangedMessage>(m =>
                {
                    return m.PageId == pageId
                        && m.HasPublishedVersionChanged == isPublished
                        && m.OldFullUrlPath == $"/{uniqueDataSlug}/{uniqueDataSlug}";
                })
                .Should().Be(1);
        }
    }
}
