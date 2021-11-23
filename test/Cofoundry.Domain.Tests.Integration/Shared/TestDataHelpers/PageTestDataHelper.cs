using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Cofoundry.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Integration
{
    /// <summary>
    /// Used to make it easier to create pages in test fixtures.
    /// </summary>
    public class PageTestDataHelper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SeededEntities _seededEntities;

        public PageTestDataHelper(
            IServiceProvider serviceProvider,
            SeededEntities seededEntities
            )
        {
            _serviceProvider = serviceProvider;
            _seededEntities = seededEntities;
        }

        /// <summary>
        /// Adds an unpublished custom entity details page that is parented 
        /// to the the specified <paramref name="parentDirectoryId"/>. The
        /// page will use the test custom entity details template which
        /// references the <see cref="TestCustomEntityDefinition"/>.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Name and UrlSlug property. 
        /// </param>
        /// <param name="parentDirectoryId">
        /// The database id of the page directory to use as the parent
        /// directory.
        /// </param>
        /// <param name="configration">
        /// Optional additional configuration action to run before the
        /// command is executed.
        /// </param>
        /// <returns>The PageDirectoryId of the newly created page.</returns>
        public async Task<int> AddAsync(
            string uniqueData,
            int parentDirectoryId,
            Action<AddPageCommand> configration = null
            )
        {
            var command = CreateAddCommand(uniqueData, parentDirectoryId);

            if (configration != null)
            {
                configration(command);
            }

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            return await contentRepository
                .Pages()
                .AddAsync(command);
        }

        /// <summary>
        /// Adds an unpublished page that is parented to the the specified
        /// <paramref name="parentDirectoryId"/>.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Name and UrlSlug property. 
        /// </param>
        /// <param name="parentDirectoryId">
        /// The database id of the page directory to use as the parent
        /// directory.
        /// </param>
        /// <param name="configration">
        /// Optional additional configuration action to run before the
        /// command is executed.
        /// </param>
        /// <returns>The PageDirectoryId of the newly created page.</returns>
        public async Task<int> AddCustomEntityPageDetailsAsync(
            string uniqueData,
            int parentDirectoryId,
            Action<AddPageCommand> configration = null
            )
        {
            var command = CreateAddCommandWithCustomEntityDetailsPage(uniqueData, parentDirectoryId);

            if (configration != null)
            {
                configration(command);
            }

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            return await contentRepository
                .Pages()
                .AddAsync(command);
        }

        /// <summary>
        /// Adds a draft version to the specified page and returns the version id.
        /// </summary>
        /// <param name="pageId">Id of the page to add a draft to.</param>
        /// <returns>The PageVersionId of the newly created version.</returns>
        public async Task<int> AddDraftAsync(int pageId)
        {
            var command = new AddPageDraftVersionCommand()
            { 
                PageId = pageId
            };

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            return await contentRepository
                .Pages()
                .Versions()
                .AddDraftAsync(command);
        }

        /// <summary>
        /// Publishes the current draft version of a page.
        /// </summary>
        /// <param name="pageId">Id of the page to publish.</param>
        public async Task PublishAsync(int pageId)
        {
            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            await contentRepository
                .Pages()
                .PublishAsync(new PublishPageCommand(pageId));
        }

        /// <summary>
        /// Creates a valid <see cref="AddPageCommand"/> using a generic page
        /// template and without the option to publish.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Title and UrlSlug property. 
        /// </param>
        /// <param name="parentDirectoryId">
        /// The database id of the page directory to use as the parent
        /// directory.
        /// </param>
        public AddPageCommand CreateAddCommand(string uniqueData, int parentDirectoryId)
        {
            var command = new AddPageCommand()
            {
                Title = uniqueData,
                PageDirectoryId = parentDirectoryId,
                UrlPath = SlugFormatter.ToSlug(uniqueData),
                PageType = PageType.Generic,
                PageTemplateId = _seededEntities.TestPageTemplate.PageTemplateId
            };

            return command;
        }

        /// <summary>
        /// Creates a valid <see cref="AddPageCommand"/> using a custom entity
        /// details page template and without the option to publish. The
        /// page will use the test custom entity details template which
        /// references the <see cref="TestCustomEntityDefinition"/>.
        /// </summary>
        /// <param name="uniqueData">
        /// Unique data to use in creating the Title and UrlSlug property. 
        /// </param>
        /// <param name="parentDirectoryId">
        /// The database id of the page directory to use as the parent
        /// directory.
        /// </param>
        public AddPageCommand CreateAddCommandWithCustomEntityDetailsPage(string uniqueData, int parentDirectoryId)
        {
            var command = new AddPageCommand()
            {
                Title = uniqueData,
                PageDirectoryId = parentDirectoryId,
                PageType = PageType.CustomEntityDetails,
                PageTemplateId = _seededEntities.TestCustomEntityPageTemplate.PageTemplateId,
                CustomEntityRoutingRule = new IdAndUrlSlugCustomEntityRoutingRule().RouteFormat
            };

            return command;
        }

        /// <summary>
        /// Adds a block of the type associated with the specified data model
        /// to a page version.
        /// </summary>
        /// <param name="pageVersionId">A page version to add the block to.</param>
        /// <param name="pageTemplateRegionId">
        /// The region of the template to add the block to. The block is appended to the end of the region.
        /// </param>
        /// <param name="dataModel">The data to add to the block type.</param>
        /// <param name="configration">
        /// Optional additional configuration action to run before the
        /// command is executed.
        /// </param>
        /// <returns>The PageVersionBlockId of the newly created block.</returns>
        public async Task<int> AddBlockAsync<TDataModel>(
            int pageVersionId, 
            int pageTemplateRegionId, 
            TDataModel dataModel, 
            Action<AddPageVersionBlockCommand, PageBlockTypeSummary> configuration = null
            )
            where TDataModel : IPageBlockTypeDataModel
        {
            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            var allBlocks = await contentRepository
                .PageBlockTypes()
                .GetAll()
                .AsSummaries()
                .ExecuteAsync();

            var fileName = dataModel.GetType().Name.Replace("DataModel", string.Empty);
            var blockType = allBlocks.SingleOrDefault(b => b.FileName == fileName);
            var command = new AddPageVersionBlockCommand()
            {
                DataModel = dataModel,
                PageBlockTypeId = blockType.PageBlockTypeId,
                PageTemplateRegionId = pageTemplateRegionId,
                PageVersionId = pageVersionId
            };

            if (configuration != null)
            {
                configuration(command, blockType);
            }

            return await contentRepository
                .Pages()
                .Versions()
                .Regions()
                .Blocks()
                .AddAsync(command);
        }

        /// <summary>
        /// Adds a simple plain text block to a page that uses the generic test template.
        /// </summary>
        /// <returns>The PageVersionBlockId of the newly created block.</returns>
        public async Task<int> AddPlainTextBlockToTestTemplateAsync(int pageVersionId, string text = "Test Text")
        {
            var template = _seededEntities.TestPageTemplate;

            return await AddBlockAsync(pageVersionId, template.BodyPageTemplateRegionId, new PlainTextDataModel()
            {
                PlainText = text
            });
        }

        /// <summary>
        /// Adds an image block to a page that uses the generic test template.
        /// </summary>
        /// <returns>The PageVersionBlockId of the newly created block.</returns>
        public async Task<int> AddImageTextBlockToTestTemplateAsync(int pageVersionId)
        {
            var template = _seededEntities.TestPageTemplate;

            return await AddBlockAsync(pageVersionId, template.BodyPageTemplateRegionId, new ImageDataModel()
            {
                ImageId = _seededEntities.TestImageId,
                AltText = "Test Alt Text"
            });
        }

        /// <summary>
        /// Adds access rule with an action of <see cref="AccessRuleViolationAction.Error"/>
        /// </summary>
        /// <param name="pageId">Id of the page to add the rule to.</param>
        /// <param name="userAreaCode">
        /// Unique 3 character code representing the user area to restrict
        /// the page to. This cannot be the Cofoundry admin user area, as 
        /// access rules do not apply to admin panel users.
        /// </param>
        /// <param name="roleId">
        /// Optionally restrict access to a specific role within the selected 
        /// user area.
        /// </param>
        /// <param name="configration">
        /// Optional additional configuration action to run before the
        /// command is executed.
        /// </param>
        public async Task AddAccessRuleAsync(
            int pageId,
            string userAreaCode,
            int? roleId = null,
            Action<UpdatePageAccessRuleSetCommand> configration = null
            )
        {
            var command = new UpdatePageAccessRuleSetCommand()
            {
                PageId = pageId,
                ViolationAction = AccessRuleViolationAction.Error
            };

            command.AccessRules.AddNew(userAreaCode, roleId);


            if (configration != null)
            {
                configration(command);
            }

            using var scope = _serviceProvider.CreateScope();
            var contentRepository = scope
                .ServiceProvider
                .GetRequiredService<IAdvancedContentRepository>()
                .WithElevatedPermissions();

            await contentRepository
                .Pages()
                .AccessRules()
                .UpdateAsync(command);
        }
    }
}
