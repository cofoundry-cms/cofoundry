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
        private readonly DbDependentFixture _dbDependentFixture;

        public PageTestDataHelper(DbDependentFixture dbDependentFixture)
        {
            _dbDependentFixture = dbDependentFixture;
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
        /// <returns>The PageDirectoryId of the newly created directory.</returns>
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

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
                PageTemplateId = _dbDependentFixture.SeededEntities.TestPageTemplate.PageTemplateId
            };

            return command;
        }

        /// <summary>
        /// Creates a valid <see cref="AddPageCommand"/> using a custom entity
        /// details page template and without the option to publish.
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
                PageTemplateId = _dbDependentFixture.SeededEntities.TestCustomEntityPageTemplate.PageTemplateId,
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
            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

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
        public async Task<int> AddPlainTextBlockToTestTemplateAsync(int pageVersionId, string text = "Test Text")
        {
            var template = _dbDependentFixture.SeededEntities.TestPageTemplate;

            return await AddBlockAsync(pageVersionId, template.BodyPageTemplateRegionId, new PlainTextDataModel()
            {
                PlainText = text
            });
        }

        /// <summary>
        /// Adds an image block to a page that uses the generic test template.
        /// </summary>
        public async Task<int> AddImageTextBlockToTestTemplateAsync(int pageVersionId)
        {
            var template = _dbDependentFixture.SeededEntities.TestCustomEntityPageTemplate;

            return await AddBlockAsync(pageVersionId, template.BodyPageTemplateRegionId, new ImageDataModel()
            {
                ImageId = _dbDependentFixture.SeededEntities.TestImageId,
                AltText = "Test Alt Text"
            });
        }
    }
}
