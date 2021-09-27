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
    [Collection(nameof(DbDependentFixture))]
    public class UpdatePageUrlCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "UpdPageUrlCHT ";
        private readonly TestDataHelper _testDataHelper;

        private readonly DbDependentFixture _dbDependentFixture;

        public UpdatePageUrlCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task CanChangeDirectory()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanChangeDirectory);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var newDirectoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData + " Copy");
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var command = new UpdatePageUrlCommand()
            {
                PageId = pageId,
                PageDirectoryId = newDirectoryId,
                UrlPath = "moved-to-new-directory"
            };

            await contentRepository
                .Pages()
                .UpdateUrlAsync(command);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var page = await dbContext
                .Pages
                .AsNoTracking()
                .FilterActive()
                .FilterByPageId(pageId)
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
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var addPageCommand = _testDataHelper.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            var page = await dbContext
                .Pages
                .AsNoTracking()
                .FilterActive()
                .FilterByPageId(pageId)
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
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var addPageCommand = _testDataHelper.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var addPage1Command = _testDataHelper.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);
            var addPage2Command = _testDataHelper.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);
            addPage2Command.CustomEntityRoutingRule = new IdCustomEntityRoutingRule().RouteFormat;

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var page1Id = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);
            var page2Id = await _testDataHelper.Pages().AddAsync(uniqueData + "2", directoryId);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var command = new UpdatePageUrlCommand()
            {
                PageId = page2Id,
                PageDirectoryId = directoryId,
                UrlPath = SlugFormatter.ToSlug(uniqueData)
            };

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
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = isPublished);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var dbContext = scope.GetRequiredService<CofoundryDbContext>();

            await contentRepository
                .Pages()
                .UpdateUrlAsync(new UpdatePageUrlCommand()
                {
                    PageId = pageId,
                    PageDirectoryId = directoryId,
                    UrlPath = "copy"
                });

            scope
                .CountMessagesPublished<PageUrlChangedMessage>(m => m.PageId == pageId && m.HasPublishedVersionChanged == isPublished)
                .Should().Be(1);
        }
    }
}
