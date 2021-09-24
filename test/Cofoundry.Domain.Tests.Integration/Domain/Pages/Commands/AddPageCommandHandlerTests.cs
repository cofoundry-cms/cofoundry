using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Commands
{
    [Collection(nameof(DbDependentFixture))]
    public class AddPageCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "AddPageCHT ";
        private readonly TestDataHelper _testDataHelper;

        private readonly DbDependentFixture _dbDependentFixture;

        public AddPageCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task WhenMinimalData_Adds()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenMinimalData_Adds);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();
            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .Include(p => p.PageVersions)
                .FilterActive()
                .FilterByPageId(addPageCommand.OutputPageId)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            var pageVersion = page.PageVersions.FirstOrDefault();

            using (new AssertionScope())
            {
                addPageCommand.OutputPageId.Should().BePositive();
                page.Should().NotBeNull();
                page.PageId.Should().Be(addPageCommand.OutputPageId);
                page.UrlPath.Should().Be(SlugFormatter.ToSlug(uniqueData));
                page.PageDirectoryId.Should().Be(addPageCommand.PageDirectoryId);
                page.LocaleId.Should().BeNull();
                page.PageTypeId.Should().Be((int)PageType.Generic);
                page.PublishDate.Should().BeNull();
                page.PublishStatusCode.Should().Be(PublishStatusCode.Unpublished);
                page.CreateDate.Should().NotBeDefault();
                page.PageVersions.Should().HaveCount(1);

                pageVersion.Should().NotBeNull();
                pageVersion.Title.Should().Be(addPageCommand.Title);
                pageVersion.PageTemplateId.Should().Be(addPageCommand.PageTemplateId);
                pageVersion.DisplayVersion.Should().Be(1);
                pageVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Draft);
                pageVersion.PageVersionId.Should().BePositive();
                pageVersion.MetaDescription.Should().BeEmpty();
                pageVersion.OpenGraphDescription.Should().BeNull();
                pageVersion.OpenGraphImageId.Should().BeNull();
                pageVersion.OpenGraphTitle.Should().BeNull();
                pageVersion.ExcludeFromSitemap.Should().Be(!addPageCommand.ShowInSiteMap);
                pageVersion.CreateDate.Should().NotBeDefault();
            }
        }

        [Fact]
        public async Task WithMetaData_Adds()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithMetaData_Adds);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.MetaDescription = "Test Meta Description";
            addPageCommand.OpenGraphDescription = "Test Open Graph Description";
            addPageCommand.OpenGraphImageId = _dbDependentFixture.SeededEntities.TestImageId;
            addPageCommand.OpenGraphTitle = "Test Open Graph Title";
            addPageCommand.ShowInSiteMap = true;

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageVersions)
                .FilterActive()
                .FilterByPageId(addPageCommand.OutputPageId)
                .SingleOrDefaultAsync();

            var pageVersion = page.PageVersions.FirstOrDefault();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                pageVersion.Should().NotBeNull();
                pageVersion.MetaDescription.Should().Be(addPageCommand.MetaDescription);
                pageVersion.OpenGraphDescription.Should().Be(addPageCommand.OpenGraphDescription);
                pageVersion.OpenGraphImageId.Should().Be(addPageCommand.OpenGraphImageId);
                pageVersion.OpenGraphTitle.Should().Be(addPageCommand.OpenGraphTitle);
                pageVersion.ExcludeFromSitemap.Should().Be(!addPageCommand.ShowInSiteMap);
            }
        }

        [Fact]
        public async Task WithTags_Adds()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithTags_Adds);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.Tags.Add(_dbDependentFixture.SeededEntities.TestTag.TagText);
            addPageCommand.Tags.Add(uniqueData);

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageTags)
                .ThenInclude(pt => pt.Tag)
                .FilterActive()
                .FilterByPageId(addPageCommand.OutputPageId)
                .SingleOrDefaultAsync();

            var testTag = page.PageTags.Select(t => t.TagId == _dbDependentFixture.SeededEntities.TestTag.TagId);
            var uniqueTag = page.PageTags.Select(t => t.Tag.TagText == uniqueData);

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.PageTags.Should().HaveCount(2);
                testTag.Should().NotBeNull();
                uniqueTag.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task WhenPublished_SetsPublishedWithCurrentDate()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPublished_SetsPublishedWithCurrentDate);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var now = new DateTime(2021, 09, 15, 13, 47, 30, DateTimeKind.Utc);
            scope.MockDateTime(now);

            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.Publish = true;

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageVersions)
                .FilterActive()
                .FilterByPageId(addPageCommand.OutputPageId)
                .SingleOrDefaultAsync();

            var pageVersion = page.PageVersions.FirstOrDefault();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.PublishDate.Should().Be(now);
                page.PublishStatusCode.Should().Be(PublishStatusCode.Published);
                pageVersion.Should().NotBeNull();
                pageVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Published);
            }
        }

        [Fact]
        public async Task WhenPublishedWithDate_SetsPublishedWithSpecifiedDate()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPublishedWithDate_SetsPublishedWithSpecifiedDate);
                        var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var now = new DateTime(2021, 09, 15, 13, 47, 30, DateTimeKind.Utc);
            scope.MockDateTime(now);
            var publishDate = new DateTime(2031, 02, 11, 05, 32, 28, DateTimeKind.Utc);

            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.Publish = true;
            addPageCommand.PublishDate = publishDate;
            
            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageVersions)
                .FilterActive()
                .FilterByPageId(addPageCommand.OutputPageId)
                .SingleOrDefaultAsync();

            var pageVersion = page.PageVersions.FirstOrDefault();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.PublishDate.Should().Be(publishDate);
                page.PublishStatusCode.Should().Be(PublishStatusCode.Published);
                pageVersion.Should().NotBeNull();
                pageVersion.WorkFlowStatusId.Should().Be((int)WorkFlowStatus.Published);
            }
        }

        [Fact]
        public async Task WithCustomEntityTemplate_Adds()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WithCustomEntityTemplate_Adds);
                        var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var addPageCommand = _testDataHelper.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .FilterActive()
                .FilterByPageId(addPageCommand.OutputPageId)
                .SingleOrDefaultAsync();

            using (new AssertionScope())
            {
                page.Should().NotBeNull();
                page.PageTypeId.Should().Equals(PageType.CustomEntityDetails);
                page.CustomEntityDefinitionCode.Should().Equals(TestCustomEntityDefinition.DefinitionCode);
                page.UrlPath.Should().Equals(addPageCommand.CustomEntityRoutingRule);
            }
        }

        [Fact]
        public async Task CanAddMultipleToOneDirectory()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanAddMultipleToOneDirectory);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await _testDataHelper.Pages().AddAsync(uniqueData + " 1", directoryId);
            await _testDataHelper.Pages().AddAsync(uniqueData + " 2", directoryId);
            await _testDataHelper.Pages().AddAsync(uniqueData + " 3", directoryId);
            var addPageCommand = _testDataHelper.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var pages = await dbContext
                .Pages
                .AsNoTracking()
                .FilterActive()
                .FilterByPageDirectoryId(directoryId)
                .ToListAsync();

            pages.Should().HaveCount(4);
        }

        [Fact]
        public async Task WhenDuplicatePath_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenDuplicatePath_Throws);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            await _testDataHelper.Pages().AddAsync(uniqueData, directoryId);
            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);

            await contentRepository
                .Awaiting(r => r.Pages().AddAsync(addPageCommand))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException>()
                .WithMemberNames(nameof(addPageCommand.UrlPath));
        }

        [Fact]
        public async Task WhenCustomEntityPageTypeWithGenericTemplate_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenCustomEntityPageTypeWithGenericTemplate_Throws);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var addPageCommand = _testDataHelper.Pages().CreateAddCommandWithCustomEntityDetailsPage(uniqueData, directoryId);
            addPageCommand.PageTemplateId = _dbDependentFixture.SeededEntities.TestPageTemplate.PageTemplateId;

            await contentRepository
                .Awaiting(r => r.Pages().AddAsync(addPageCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(addPageCommand.PageTemplateId));
        }

        [Fact]
        public async Task WhenGenericPageTypeWithCustomEntityTemplate_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenGenericPageTypeWithCustomEntityTemplate_Throws);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.PageTemplateId = _dbDependentFixture.SeededEntities.TestCustomEntityPageTemplate.PageTemplateId;

            await contentRepository
                .Awaiting(r => r.Pages().AddAsync(addPageCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(addPageCommand.PageTemplateId));
        }

        [Fact]
        public async Task WhenNotPublished_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenNotPublished_SendsMessage);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();

            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            scope
                .CountMessagesPublished<PageAddedMessage>(m => m.PageId == addPageCommand.OutputPageId && !m.HasPublishedVersionChanged)
                .Should().Be(1);
        }

        [Fact]
        public async Task WhenPublished_SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(WhenPublished_SendsMessage);
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();

            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();
            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.Publish = true;

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            scope
                .CountMessagesPublished<PageAddedMessage>(m => m.PageId == addPageCommand.OutputPageId && m.HasPublishedVersionChanged)
                .Should().Be(1);
        }
    }
}
