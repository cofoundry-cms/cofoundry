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

namespace Cofoundry.Domain.Tests.Integration
{
    [Collection(nameof(DbDependentFixture))]
    public class AddPageCommandHandlerTests
    {
        const string DATA_PREFIX = "AddPageCHT ";

        private readonly DbDependentFixture _dbDependentFixture;

        public AddPageCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
        }

        [Fact]
        public async Task WhenMinimalData_Adds()
        {
            var uniqueData = DATA_PREFIX + nameof(WhenMinimalData_Adds);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();
            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .Include(p => p.PageVersions)
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
            var uniqueData = DATA_PREFIX + nameof(WithMetaData_Adds);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();
            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);
            addPageCommand.MetaDescription = "Test Meta Description";
            addPageCommand.OpenGraphDescription = "Test Open Graph Description";
            addPageCommand.OpenGraphImageId = _dbDependentFixture.SeededEntities.TestImageId;
            addPageCommand.OpenGraphTitle = "Test Open Graph Title";
            addPageCommand.ShowInSiteMap = true;

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageVersions)
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
            var uniqueData = DATA_PREFIX + nameof(WithTags_Adds);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();

            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);
            addPageCommand.Tags.Add(_dbDependentFixture.SeededEntities.TestTag);
            addPageCommand.Tags.Add(uniqueData);

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageTags)
                .ThenInclude(pt => pt.Tag)
                .FilterByPageId(addPageCommand.OutputPageId)
                .SingleOrDefaultAsync();

            var testTag = page.PageTags.Select(t => t.TagId == _dbDependentFixture.SeededEntities.TestTagId);
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
            var uniqueData = DATA_PREFIX + nameof(WhenPublished_SetsPublishedWithCurrentDate);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();
            var now = new DateTime(2021, 09, 15, 13, 47, 30, DateTimeKind.Utc);
            scope.MockDateTime(now);

            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);
            addPageCommand.Publish = true;

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageVersions)
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
            var uniqueData = DATA_PREFIX + nameof(WhenPublishedWithDate_SetsPublishedWithSpecifiedDate);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();
            var now = new DateTime(2021, 09, 15, 13, 47, 30, DateTimeKind.Utc);
            scope.MockDateTime(now);
            var publishDate = new DateTime(2031, 02, 11, 05, 32, 28, DateTimeKind.Utc);

            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);
            addPageCommand.Publish = true;
            addPageCommand.PublishDate = publishDate;

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageVersions)
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
            var uniqueData = DATA_PREFIX + nameof(WithCustomEntityTemplate_Adds);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();

            var addPageCommand = CreateValidCustomEntityDetailsPageCommand(_dbDependentFixture, uniqueData, directoryId);

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            var dbContext = scope.GetRequiredService<CofoundryDbContext>();
            var page = await dbContext
                .Pages
                .AsNoTracking()
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
        public async Task WhenDuplicatePath_Throws()
        {
            var uniqueData = DATA_PREFIX + nameof(WhenDuplicatePath_Throws);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);

            await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);

            await contentRepository
                .Awaiting(r => r.Pages().AddAsync(addPageCommand))
                .Should()
                .ThrowAsync<UniqueConstraintViolationException>()
                .WithMemberNames(nameof(addPageCommand.UrlPath));
        }

        [Fact]
        public async Task WhenCustomEntityPageType_UsingGenericTemplateThrows()
        {
            var uniqueData = DATA_PREFIX + nameof(WhenCustomEntityPageType_UsingGenericTemplateThrows);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var addPageCommand = CreateValidCustomEntityDetailsPageCommand(_dbDependentFixture, uniqueData, directoryId);
            addPageCommand.PageTemplateId = _dbDependentFixture.SeededEntities.TestPageTemplateId;

            await contentRepository
                .Awaiting(r => r.Pages().AddAsync(addPageCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(addPageCommand.PageTemplateId));
        }

        [Fact]
        public async Task WhenGenericPageType_UsingCustomEntityTemplateThrows()
        {
            var uniqueData = DATA_PREFIX + nameof(WhenGenericPageType_UsingCustomEntityTemplateThrows);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);
            addPageCommand.PageTemplateId = _dbDependentFixture.SeededEntities.TestCustomEntityPageTemplateId;

            await contentRepository
                .Awaiting(r => r.Pages().AddAsync(addPageCommand))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMemberNames(nameof(addPageCommand.PageTemplateId));
        }

        [Fact]
        public async Task WhenNotPublished_SendsMessage()
        {
            var uniqueData = DATA_PREFIX + nameof(WhenNotPublished_SendsMessage);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();

            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();
            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            scope
                .CountMessagesPublished<PageAddedMessage>(m => m.PageId == addPageCommand.OutputPageId && !m.HasPublishedVersionChanged)
                .Should().Be(1);
        }

        [Fact]
        public async Task WhenPublished_SendsMessage()
        {
            var uniqueData = DATA_PREFIX + nameof(WhenPublished_SendsMessage);
            var directoryId = await AddPageDirectoryAsync(_dbDependentFixture, uniqueData);

            using var scope = _dbDependentFixture.CreateServiceScope();

            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();
            var addPageCommand = CreateValidCommand(_dbDependentFixture, uniqueData, directoryId);
            addPageCommand.Publish = true;

            await contentRepository
                .WithElevatedPermissions()
                .Pages()
                .AddAsync(addPageCommand);

            scope
                .CountMessagesPublished<PageAddedMessage>(m => m.PageId == addPageCommand.OutputPageId && m.HasPublishedVersionChanged)
                .Should().Be(1);
        }

        private static async Task<int> AddPageDirectoryAsync(DbDependentFixture dbDependentFixture, string uniqueData)
        {
            using var scope = dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetRequiredService<IAdvancedContentRepository>();

            var command = await AddPageDirectoryCommandHandlerTests.CreateValidCommandWithRootParentDirectoryAsync(uniqueData, dbDependentFixture);

            return await contentRepository
                .WithElevatedPermissions()
                .PageDirectories()
                .AddAsync(command);
        }

        public static AddPageCommand CreateValidCommand(DbDependentFixture dbDependentFixture, string uniqueData, int parentDirectoryId)
        {
            var command = new AddPageCommand()
            {
                Title = uniqueData,
                PageDirectoryId = parentDirectoryId,
                UrlPath = SlugFormatter.ToSlug(uniqueData),
                PageType = PageType.Generic,
                PageTemplateId = dbDependentFixture.SeededEntities.TestPageTemplateId
            };

            return command;
        }

        public static AddPageCommand CreateValidCustomEntityDetailsPageCommand(DbDependentFixture dbDependentFixture, string uniqueData, int parentDirectoryId)
        {
            var command = new AddPageCommand()
            {
                Title = uniqueData,
                PageDirectoryId = parentDirectoryId,
                PageType = PageType.CustomEntityDetails,
                PageTemplateId = dbDependentFixture.SeededEntities.TestCustomEntityPageTemplateId,
                CustomEntityRoutingRule = new IdAndUrlSlugCustomEntityRoutingRule().RouteFormat
            };

            return command;
        }
    }
}
